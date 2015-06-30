(function () {
    var module = appMainPlugin.getAngularModule();
    module.filter('kbToGB', function () {
        return function (text, length, end) {
            if (text == null || text == '') text = '0';
            text = Math.round(parseInt(text, 10) / 1024);
            return text;
        };
    });

    module.filter('toDate', function () {
        return function (text, length, end) {
            if (text == null || text == '' || text.length < 19)
                return 'Invalid';
            var day = text.slice(8, 10);
            var month = text.slice(5, 7);
            var year = text.slice(0, 4);
            var hour = text.slice(11, 13);
            var minute = text.slice(14, 16);
            var second = text.slice(17, 19);
            return day + '.' + month + '.' + year + ' ' + hour + ':' + minute + ':' + second;
        };
    });

    module.filter('toTimespan', function () {
        return function (text, length, end) {
            if (text == null || text == '') text = '0';
            var d = Number(parseInt(text, 10));
            var years = Math.floor(d / 31536000);
            var days = Math.floor(d / 86400 % 365);
            var h = Math.floor(d / 3600 % 24);
            var m = Math.floor(d % 3600 / 60);
            var s = Math.floor(d % 3600 % 60);
            var timeStr = "";
            if (years > 0) {
                timeStr = years + "y ";
            }
            if (days > 0) {
                timeStr = timeStr + days + "d ";
            }
            return timeStr + h.zeropad(2) + ":" + m.zeropad(2) + ":" + s.zeropad(2);
        };
    });
})();