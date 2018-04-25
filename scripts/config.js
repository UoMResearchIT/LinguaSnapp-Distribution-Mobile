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

    // App specific text, e.g. title etc
    this.appText = {
        "app-title": "LinguaSnappHamburg",
        "app-emailstudent": "Falls Du Studierende/r der Universität Hamburg bist und LinguaSnappHamburg im Rahmen einer Lehrveranstaltung nutzt, trage bitte Deine Uni-Email ein, damit wir Dich identifizieren können.",
        "app-location": "Hamburg"

    }

    

}