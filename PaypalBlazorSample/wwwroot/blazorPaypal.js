window.blazorPaypal = (approval_url) => {
    var ppp = PAYPAL.apps.PPP({
        "approvalUrl": approval_url,
        "placeholder": "ppplus",
        "mode": "sandbox",
        "country": "DE"
    });
    console.log("approval_url=" + approval_url); 
}