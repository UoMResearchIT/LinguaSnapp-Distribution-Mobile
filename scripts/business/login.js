// Class:   
//          login
//          Login functionality
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.

var Login = function (db) {


    this.linguaDB = db;

    this.configDB = new ConfigDB(this.linguaDB);
    this.phoneGapUtils = new PhoneGapUtils();

    //-------------------------------------------------------------------------------------------------------------
 
    // sendLoginInformation()
    //
    // Contact the servers to register.
    this.sendLoginInformation = function (usercode, password) {
        var deferred = new $.Deferred();
        var self = this;
        var userId = "";
        var deviceId = "";

        $.mobile.loading('show', {
            text: "Logging in. Please wait.",
            textVisible: true

        });

        // check connected
        var network_promise = this.phoneGapUtils.isNetworkConnected();
        network_promise.fail(function () { // not connected
            deferred.reject();
        });

        network_promise.done(function () { //connected
   
            var uuid = self.phoneGapUtils.getGUID(); // by default select a random number
            if (typeof device !== 'undefined') {
                uuid = device.uuid; // Find the device UUID
            }

    
            // Get the current app version
            version_promise = self.configDB.getKey("version");
            version_promise.fail(function () {
                deferred.reject();
            });

            version_promise.done(function (versionData) {

                // Send the registration info
                var promise = $.ajax({
                    url: self.configDB.serverLocation + "/api/login",
                    accepts: "application/json",
                    contentType: "application/json",
                    dataType: "json",
                    method: "POST",
                    crossDomain: true,
                    data: JSON.stringify({
                        "deviceUUID": uuid,
                        "usercode": usercode,
                        "password": password,
                        "version": versionData.value,
                        "EOT": true
                    })
                });


                // Successful communication
                promise.done(function (data) {


                    // LinguaSnapp error message returned
                    if (data.Code >= 200 && data.Code <= 299) {

                        if (data.Code === 210) {
                            // User already exists. Prompt a possible alternative.
                            data.Message = data.Message + " Try an alternative such as " + data.Details + ".";
                        }

                        self.phoneGapUtils.showAlert(data.Message);
                        deferred.reject();
                    }
                    else { // No error - registration successfully done on server so update user id.
                        var returnedCodes = data.Details.split(":"); // Details contain user and device id separated by colon
                        userId = returnedCodes[0];
                        deviceId = returnedCodes[1];

                        var promise2 = self.configDB.updateValue("userid", userId);
                        // If updated user id successfully then update the device id
                        promise2.done(function () {
                            var promiseDev = self.configDB.updateValue("deviceid", deviceId);
                            // If updated device id successfully then update the registered flag
                            promiseDev.done(function () {
                                var promise3 = self.configDB.updateValue("registered", "yes");
                                // If updated registered flag successfully then send confirmed message
                                promise3.done(function () {
                                    // Send the registration info
                                    var promise4 = $.ajax({
                                        url: self.configDB.serverLocation + "/api/confirmconfig",
                                        accepts: "application/json",
                                        contentType: "application/json",
                                        dataType: "json",
                                        method: "POST",
                                        crossDomain: true,
                                        data: JSON.stringify({
                                            "deviceUUID": deviceId,
                                            "userUUID": userId,
                                            "confirmed": true,
                                            "EOT": true
                                        })
                                    });

                                    // Could not contact server
                                    promise4.fail(function (data) {
                                        self.phoneGapUtils.showAlert("There has been an error contacting the LinguaSnapp servers. Please try again later.");
                                        deferred.reject();
                                    });

                                    promise4.done(function (data) {
                                        $("#aboutBack").attr("href", "#main");
                                        $(':mobile-pagecontainer').pagecontainer("change", "#main", { changeHash: false });
                                        deferred.resolve();
                                    });

                                });

                                // If failed to update registered flag reject
                                promise3.fail(function () {
                                    deferred.reject();
                                });
                            });

                            promiseDev.fail(function () { // Failed to update device ID
                                deferred.reject();
                            });

                        });

                        // If failed to update user id reject
                        promise2.fail(function () {
                            deferred.reject();
                        });


                    }
                });

                // Could not contact server
                promise.fail(function (data) {
                    self.phoneGapUtils.showAlert("There has been an error contacting the LinguaSnapp servers. Please try again later.");
                    deferred.reject();
                });
            });
        });

        return deferred.promise();
    }

}