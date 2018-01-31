var appLoginPlugin = app.registerPlugin('login');
(function () {
    var plugin = appLoginPlugin;
    plugin.dependencies = ['ngResource', 'ui.bootstrap'];
    plugin.files = [
        'loginCtrl.js',
        'login.css'
    ];
    plugin.view = 'login.cshtml';
    plugin.controller = 'app.login.ctrl';
    plugin.routes = [];
})();