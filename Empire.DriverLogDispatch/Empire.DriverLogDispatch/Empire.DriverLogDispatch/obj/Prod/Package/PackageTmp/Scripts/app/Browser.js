function isTablet() {
    const userAgent = navigator.userAgent.toLowerCase();
    return /(ipad|tablet|(android(?!.*mobile))|(windows(?!.*phone)(.*touch))|kindle|playbook|silk|(puffin(?!.*(IP|AP|WP))))/.test(userAgent);
}


function resizeLogo() {

    let width = isTablet() ? '30%' : 'auto';
    let height = isTablet() ? '30%' : 'auto';

    $('#imgLogo').css("width", width);
    $('#imgLogo').css("height", height);

}