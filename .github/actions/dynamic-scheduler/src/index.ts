import * as core from '@actions/core';
import * as github from '@actions/github';
import axios from 'axios';

interface SchedulerResponse {
  jobNames: string[];
  workflowPath: string;
  content?: string;
  skip?: {
    step1: boolean;
    step2: boolean;
    step3: boolean;
  };
}

async function run(): Promise<void> {
  try {
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

    // Remove empty or null values
    Object.keys(requestBody).forEach(key => {
      if (requestBody[key] === undefined || requestBody[key] === null || requestBody[key] === '') {
        delete requestBody[key];
      }
    });

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

    // Set skip flags if provided, otherwise set defaults
    const skipFlags = response.data.skip || {
      step1: false,
      step2: false,
      step3: false
    };
    core.setOutput('skip', JSON.stringify(skipFlags));

    if (debug && response.data.content) {
      core.setOutput('content', response.data.content);
    }

    // Log response for debugging
    if (debug) {
      core.info('API Response:');
      core.info(JSON.stringify(response.data, null, 2));
    }

  } catch (error) {
    if (error instanceof Error) {
      core.setFailed(error.message);
    } else {
      core.setFailed('An unknown error occurred');
    }
  }
}

run(); 