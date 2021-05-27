'use strict';

$(document).ready(function () {

    resizeLogo();
    let padding = isTablet() ? '60px' : '350px';

    $('#mainDiv').css("padding-left", padding);
});

