function isTablet() {
    const userAgent = navigator.userAgent.toLowerCase();
    return /(ipad|tablet|(android(?!.*mobile))|(windows(?!.*phone)(.*touch))|kindle|playbook|silk|(puffin(?!.*(IP|AP|WP))))/.test(userAgent);
}


function resizeLogo() {

    let width = isTablet() ? '30%' : '500px';
    let height = isTablet() ? '30%' : '90px';

    $('#imgLogo').css("width", width);
    $('#imgLogo').css("height", height);

}