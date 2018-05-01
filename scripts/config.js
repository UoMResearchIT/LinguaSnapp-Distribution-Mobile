// Class:   
//          config
//          Configuration settings for the app publisher to modify.
//          NB: This is Javascript!

var Config = function () {

    // This is the webservice location.
    this.serverLocation = "https://api.linguasnapp.com";
   

    // This is the web application location (used on the about page)
    this.mapLocation = "http://www.linguasnapp.com";

    // The app version
    this.appVersion = "1.0.0";

    // App specific text, e.g. title etc. Explained in the installation instructions!
    this.appText = {
        "app-title": "LinguaSnapp",
        "app-emailstudent": "If you are a University of Manchester student and have been asked to use this app as part of your course unit then please enter your university email address to allow us to identify you for your group work.",
        "app-location": "Manchester"

    }

    

}