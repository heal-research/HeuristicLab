(function () {
    var module = appStatisticsPlugin.getAngularModule();
    var apiUrl = 'api/Statistics/Job/';
    module.factory('app.statistics.jobService',
        ['$resource', function ($resource) {
            return $resource(apiUrl + ':action', { id: '@id', page: '@page', size: '@size', states: '@states', completed: '@completed' }, {
                getJobDetails: { method: 'GET', params: { action: 'GetJobDetails' } },
                getJobs: { method: 'GET', params: { action: 'GetJobs' } },
                getAllJobs: { method: 'GET', params: { action: 'GetAllJobs'}, isArray: true },
                getJobsByUserId: { method: 'GET', params: { action: 'GetJobsByUserId' } },
                getAllJobsByUserId: { method: 'GET', params: { action: 'GetAllJobsByUserId'}, isArray: true },
                getJobTasksStatesByJobId: { method: 'GET', params: { action: 'GetJobTasksStatesByJobId' }, isArray: true },
                getAllActiveJobsFromAllUsers: { method: 'GET', params: { action: 'GetAllActiveJobsFromAllUsers' }, isArray: true }
            });
        }]
    );
})();