import * as core from '@actions/core';
import * as github from '@actions/github';
import axios from 'axios';

interface SchedulerResponse {
  jobNames: string[];
  workflowPath: string;
  content?: string;
  skip: Record<string, boolean>;
}

async function hasExistingComment(octokit: ReturnType<typeof github.getOctokit>, prNumber: number): Promise<boolean> {
  const context = github.context;
  const comments = await octokit.rest.issues.listComments({
    owner: context.repo.owner,
    repo: context.repo.repo,
    issue_number: prNumber
  });

  return comments.data.some(comment => 
    comment.user?.login === 'github-actions[bot]' && 
    comment.body?.includes('Dynamic Scheduler Error')
  );
}

async function postPRComment(message: string): Promise<void> {
  const token = core.getInput('github_token');
  if (!token) {
    return; // Skip if no token provided
  }

  const octokit = github.getOctokit(token);
  const context = github.context;

  if (context.payload.pull_request) {
    try {
      // Check if we already have a comment
      const hasComment = await hasExistingComment(octokit, context.payload.pull_request.number);
      
      if (!hasComment) {
        await octokit.rest.issues.createComment({
          owner: context.repo.owner,
          repo: context.repo.repo,
          issue_number: context.payload.pull_request.number,
          body: message
        });
      }
    } catch (error) {
      core.warning(`Failed to post PR comment: ${error instanceof Error ? error.message : 'Unknown error'}`);
    }
  }
}

async function run(): Promise<void> {
  // Get inputs
  const workflowFile = core.getInput('workflow_file');
  const debug = core.getBooleanInput('debug');
  const workflowName = github.context.workflow;
  
  // Get repository name from context
  const repositoryName = github.context.repo.repo;
  const repositoryOwner = github.context.repo.owner;
  const fullRepositoryName = `${repositoryOwner}/${repositoryName}`;
  
  // Get commit SHA from context
  const commitSha = github.context.sha;

  // Get pull request number from context
  const pullRequestNumber = github.context.payload.pull_request?.number;

  // Construct request body
  const requestBody: Record<string, any> = {
    repository_name: fullRepositoryName,
    commit_sha: commitSha,
    debug
  };

  if (workflowFile) {
    requestBody.workflow_file = workflowFile;
  } else {
    requestBody.workflow_name = workflowName;
  }

  if (pullRequestNumber) {
    requestBody.pull_request_number = pullRequestNumber;
  }

  // Remove empty or null values
  Object.keys(requestBody).forEach(key => {
    if (requestBody[key] === undefined || requestBody[key] === null || requestBody[key] === '') {
      delete requestBody[key];
    }
  });

  // Set default outputs in case of failure
  core.setOutput('jobNames', '');
  core.setOutput('workflowPath', '');
  core.setOutput('skip', JSON.stringify({}));

  try {
    // Make API call
    const response = await axios.post<SchedulerResponse>(
      'http://localhost:3000/api/dynamic-scheduler',
      requestBody,
      {
        headers: {
          'Content-Type': 'application/json'
        }
      }
    );

    // Set outputs
    core.setOutput('jobNames', response.data.jobNames.join('\n'));
    core.setOutput('workflowPath', response.data.workflowPath);
    core.setOutput('skip', JSON.stringify(response.data.skip));

    if (debug && response.data.content) {
      core.setOutput('workflow_file_content', response.data.content);
    }

    // Log response for debugging
    if (debug) {
      core.info('API Response:');
      core.info(JSON.stringify(response.data, null, 2));

      core.info('Skip Json Info:');
      core.info(JSON.stringify(response.data.skip))
    }
  } catch (error) {
    let errorMessage = 'Dynamic scheduler service error';
    
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 403) {
        // For 403 errors, fail with access denied message
        errorMessage = error.response.data?.message || 'Access forbidden';
        if (debug) {
          core.info('Error Response:');
          core.info(JSON.stringify(error.response.data, null, 2));
        }
      } else {
        // For all other errors, fail with service error message
        errorMessage = error.response?.data?.message || 'Service error';
        if (debug) {
          core.info('Error Response:');
          core.info(JSON.stringify(error.response?.data, null, 2));
        }
      }
    }

    // Post error message to PR if this is a PR and we have a token
    await postPRComment(`❌ Dynamic Scheduler Error: ${errorMessage}`);
    
    core.setFailed(errorMessage);
  }
}

run(); 