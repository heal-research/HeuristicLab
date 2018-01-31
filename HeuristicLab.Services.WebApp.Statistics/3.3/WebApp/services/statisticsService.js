(function () {
    var module = appStatisticsPlugin.getAngularModule();
    var apiUrl = 'api/Statistics/Data/';
    module.factory('app.statistics.data.service',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { id: '@id', page: '@page', size: '@size', states: '@states', expired: '@expired'}, {
                getJob: { method: 'GET', params: { action: 'GetJob' } },
                getJobs: { method: 'GET', params: { action: 'GetJobs' }, isArray: true },
                getCompletedJobs: { method: 'GET', params: { action: 'GetCompletedJobs' }, isArray: true },
                getCompletedJobsByUserId: { method: 'GET', params: { action: 'GetCompletedJobsByUserId' }, isArray: true },
                getCompletedJobsCount: { method: 'GET', params: { action: 'GetCompletedJobsCount' } },
                getCompletedJobsCountByUserId: { method: 'GET', params: { action: 'GetCompletedJobsCountByUserId' } },
                getJobsByUserId: { method: 'GET', params: { action: 'GetJobsByUserId' }, isArray: true },
                getTaskCountByJobId: { method: 'GET', params: { action: 'GetTaskCountByJobId' } },
                getTasksByJobId: { method: 'GET', params: { action: 'GetTasksByJobId' }, isArray: true },
                getJobTasksStatesByJobId: { method: 'GET', params: { action: 'GetJobTasksStatesByJobId' }, isArray: true },
                getClient: { method: 'GET', params: { action: 'GetClient' } },
                getClientCount: { method: 'GET', params: { action: 'GetClientCount' } },
                getClients: { method: 'GET', params: { action: 'GetClients' }, isArray: true },
                getTaskCountByClientId: { method: 'GET', params: { action: 'GetTaskCountByClientId' } },
                getTasksByClientId: { method: 'GET', params: { action: 'GetTasksByClientId' }, isArray: true },
                getJobTasksStatesByClientId: { method: 'GET', params: { action: 'GetJobTasksStatesByClientId' }, isArray: true },
                getClientHistory: { method: 'GET', params: { action: 'GetClientHistory' }, isArray: true },
                getUsers: { method: 'GET', params: { action: 'GetUsers' }, isArray: true },
                getUser: { method: 'GET', params: { action: 'GetUser' } }
            });
        }]
    );
})();