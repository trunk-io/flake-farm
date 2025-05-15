import * as core from '@actions/core';
import * as github from '@actions/github';
import axios from 'axios';

interface SchedulerResponse {
  jobNames: string[];
  workflowPath: string;
  content?: string;
  skip: Record<string, boolean>;
}

async function run(): Promise<void> {
  try {
    // Get inputs
    const workflowFile = core.getInput('workflow_file');
    const debug = core.getBooleanInput('debug');
    const continueOnError = core.getBooleanInput('continue_on_error');
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
      if (axios.isAxiosError(error) && error.response?.status === 403) {
        // For 403 errors, output the error message from the service
        const errorMessage = error.response.data?.message || 'Access forbidden';
        core.error(`Service returned 403: ${errorMessage}`);
        if (!continueOnError) {
          core.setFailed(errorMessage);
        } else {
          core.warning(`Continuing despite error: ${errorMessage}`);
        }
      } else {
        // For other errors, handle based on continueOnError setting
        if (!continueOnError) {
          throw error;
        } else {
          const errorMessage = error instanceof Error ? error.message : 'Unknown error occurred';
          core.warning(`Continuing despite error: ${errorMessage}`);
        }
      }
    }

  } catch (error) {
    if (error instanceof Error) {
      core.error(error.message);
      const continueOnError = core.getBooleanInput('continue_on_error');
      if (!continueOnError) {
        core.setFailed(error.message);
      } else {
        core.warning(`Continuing despite error: ${error.message}`);
      }
    } else {
      const errorMessage = 'An unknown error occurred';
      core.error(errorMessage);
      const continueOnError = core.getBooleanInput('continue_on_error');
      if (!continueOnError) {
        core.setFailed(errorMessage);
      } else {
        core.warning(`Continuing despite error: ${errorMessage}`);
      }
    }
  }
}

run();
