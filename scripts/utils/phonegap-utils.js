// Class:   
//          PhoneGapUtils
//          Various PhoneGap related methods and properties.

var PhoneGapUtils = function () {

    var config = new Config();

    //-------------------------------------------------------------------------------------------------------------

    // isNetworkConnected()
    // Is the device connected to the network (WIFI or phone network)
    // Returns true if it is connected else false
    this.isNetworkConnected = function () {

        var deferred = $.Deferred();

        if (typeof navigator.connection !== 'undefined') {
 
            var connectionState = navigator.connection.type;

            // Not connected
            if (connectionState === Connection.NONE) {


                this.showAlert("You are not connected to the Internet");

                deferred.reject();
            }
            else { // connected
                deferred.resolve();
            }
        }
        else { // If the PhoneGap connection object isn't even defined then we must be using the emulator, so return true
            deferred.resolve();
        }

        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // showAlert()
    // Show a native alert box, if available.
    // Params:
    // message: the message to show
   this.showAlert = function(message){
        if (typeof navigator.notification !== 'undefined') {
            navigator.notification.alert(message, null);
        }
        else{
            alert(message);
           
        }
   }

    //-------------------------------------------------------------------------------------------------------------

    // showConfirm()
    // Show a native confirm box, if available.
    // Params:
    // message: the message to show
   this.showConfirm = function (message, callback) {
       if (typeof navigator.notification !== 'undefined') {
           navigator.notification.confirm(message, callback);
       }
       else {
           var conf = confirm(message);
           var confInt = (conf === true) ? 1 : 2;
           callback(confInt);

       }
   }

    //-------------------------------------------------------------------------------------------------------------

    // htmlEscape()
    // Encode the HTML.
   this.htmlEscape = function(str) {
       return String(str)
               .replace(/"/g, '&quot;')
               .replace(/'/g, '&#39;')
               .replace(/</g, '&lt;')
               .replace(/>/g, '&gt;');
   }

    //-------------------------------------------------------------------------------------------------------------

    // getGPS()
    // Get the GPS.
   this.getGPS = function () {
       var deferred = new $.Deferred();

       if (typeof navigator.geolocation !== 'undefined') {
           navigator.geolocation.getCurrentPosition(function (position) {
               // On successfully getting GPS return the data
               deferred.resolve({
                   "lat": position.coords.latitude,
                   "long": position.coords.longitude
               });
              
           },
           function (error) {
               // Did not get the GPS 
              
               deferred.reject();
           }, {
               maximumAge:0 ,
               timeout: 5000,
               enableHighAccuracy:true
           });
       }
       else
       {
           deferred.reject();
       }


       return deferred.promise();
   }


    //-------------------------------------------------------------------------------------------------------------

    // getGUID()
    // Get a random unique number.
   this.getGUID = function () {
       var unique_id = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
           var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
           return v.toString(16);
       });

       return unique_id;
   }


    //-------------------------------------------------------------------------------------------------------------

    // addAppText()
    // Add the bespoke app text to the HTML
   this.addAppText = function () {
       var self = this;

       // find spans with the data-app-text attribute
       $('span[data-app-text]').each(function () {
           var textCode = $(this).data("app-text");
           
           var textString = config.appText[textCode]; // find the text in config.js

           $(this).text(textString);
       });
   }
}