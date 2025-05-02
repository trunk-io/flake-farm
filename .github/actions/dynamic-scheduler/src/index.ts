import * as core from '@actions/core';
import * as github from '@actions/github';
import axios from 'axios';

interface SchedulerResponse {
  jobNames: string[];
  workflowPath: string;
  content?: string;
}

async function run(): Promise<void> {
  try {
    // Get inputs
    const workflowFile = core.getInput('workflow_file');
    const repositoryName = core.getInput('repository_name');
    const debug = core.getBooleanInput('debug');
    const workflowName = github.context.workflow;

    // Construct request body
    const requestBody: Record<string, any> = {
      repository_name: repositoryName || undefined,
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
      core.setFailed('An unknown error occurred - boo');
    }
  }
}

run(); 