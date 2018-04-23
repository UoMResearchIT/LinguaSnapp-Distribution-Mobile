// Class:   
//          Upload
//          Upload functionality
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.

var Upload = function (db) {


    this.linguaDB = db;

    this.configDB = new ConfigDB(this.linguaDB);
    this.config = new Config();
    this.phoneGapUtils = new PhoneGapUtils();
    this.photoRecord = new PhotoRecord(this.linguaDB);


    //-------------------------------------------------------------------------------------------------------------

    // uploadAllPhotos()
    //
    // Contact the servers to upload the passed photo.
    this.uploadAllPhotos = function (unique_id) {
        var deferred = new $.Deferred();
        var self = this;
        var userId = "";

        var get_promise = self.photoRecord.getAllRecords();

        get_promise.done(function (data) { // Got all the records
            var index = 0;
            var def = [];
            for (index = 0; index < data.length; index++) {
                if ((data[index].latitude === 999 || data[index].longitude === 999) && (data[index].comments === null || data[index].comments === "")) {
                    continue;
                }

                def.push(self.photoRecord.createPayload(data[index].unique_id));
            }

            // Wait until they have finished then resolve
            var prom = $.when.apply($, def);
            
            prom.done(function () {
                var payloads = [];
                $.each(arguments, function (index, responseData) {
                    payloads.push(responseData);
                });
                var payload_str = JSON.stringify(payloads);
                var payloadSize = Math.round(payload_str.length / 1024);

                if (payloadSize === 0) {
                    self.phoneGapUtils.showAlert("There are no pending photos to upload.");
                    deferred.resolve();
                }
                else {
                    self.phoneGapUtils.showConfirm("These photos with their data are " + payloadSize + "KiB in size. Continue upload?", function (buttonIndex) {
                        if (buttonIndex === 1) { // User said OK to upload

                            $.mobile.loading('show', {
                                text: "Uploading. Please wait.",
                                textVisible: true

                            });

                            // check connected
                            var network_promise = self.phoneGapUtils.isNetworkConnected();
                            network_promise.fail(function () { // not connected
                                $.mobile.loading('hide');
                                deferred.reject();
                            });

                            network_promise.done(function () { // connected

                                var dfd = $.Deferred(), pfd = dfd.promise(), index = 0;
                                for (index = 0; index < payloads.length; index++) {

                                    // Upload all the photos sequentially - waiting for one to finish before the next is transmitted.
                                    (function (i) {

                                        pfd = pfd.then(function () {
                                            payload_str = JSON.stringify(payloads[i]);
                                            return self.uploadPhoto(payloads[i].unique_id, payload_str);
                                        });
                                    })(index);
                                }


                                pfd = pfd.then(function () {
                                    // After all have been transmitted give a message and return.
                                    self.phoneGapUtils.showAlert("Your upload has succeeded. Thank you.");
                                    deferred.resolve();
                                });
                                dfd.resolve();

                            });

                        }
                        else {
                            // User said to cancel upload.
                            deferred.resolve();

                        }
                    });
                }
                
            });

        });

        get_promise.fail(function () {

            deferred.reject();
        });

        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // uploadSinglePhoto()
    //
    // Contact the servers to upload the passed photo.
    this.uploadSinglePhoto = function (unique_id) {
        var deferred = new $.Deferred();
        var self = this;
        var userId = "";

        // Create the payload to upload
        var payload_promise = self.photoRecord.createPayload(unique_id);

        payload_promise.done(function (payload) { // created the payload
            var payload_str = JSON.stringify(payload);
            var payloadSize = Math.round(payload_str.length / 1024);
            // Check that the payload will have a valid address if it has no GPS values.
            if ((payload.latitude === 999 || payload.longitude === 999) && (payload.comments === null || payload.comments === "")) {
                self.phoneGapUtils.showAlert("For this photo LinguaSnapp was unable to determine your location. Please enter the address in the comments box of the Context page.");
                deferred.reject();
            }
            else {



                self.phoneGapUtils.showConfirm("This photo with its data is " + payloadSize + "KiB in size. Continue upload?", function (buttonIndex) {

                    if (buttonIndex === 1) { // User said OK to upload
                        $.mobile.loading('show', {
                            text: "Uploading. Please wait.",
                            textVisible: true

                        });

                        // check connected
                        var network_promise = self.phoneGapUtils.isNetworkConnected();
                        network_promise.fail(function () { // not connected
                            $.mobile.loading('hide');
                            deferred.reject();
                        });

                        network_promise.done(function () { // connected

                            var up_prom = self.uploadPhoto(payload.unique_id, payload_str);

                            up_prom.done(function () {
                                self.phoneGapUtils.showAlert("Your upload has succeeded. Thank you.");
                                deferred.resolve();
                            });

                            up_prom.fail(function () { // Failed to upload
                                $.mobile.loading('hide');
                                deferred.reject();
                            });

                        });
                    }
                });
            }

           
        });

        payload_promise.fail(function () { // Failed to create payload
            deferred.reject();
        });
        

        return deferred.promise();
    }

    //-------------------------------------------------------------------------------------------------------------

    // uploadPhoto()
    //
    // Contact the servers to upload the passed photo.
    this.uploadPhoto = function (unique_id, payload_str) {
        var deferred = $.Deferred();
        var self = this;
        var userId = "";

        // Send the registration info
        var upload_promise = $.ajax({
            url: self.config.serverLocation + "/api/upload",
            accepts: "application/json",
            contentType: "application/json",
            dataType: "json",
            method: "POST",
            crossDomain: true,
            data: payload_str
        });

        // Successfully contacted the servers
        upload_promise.done(function (data) {
            // LinguaSnapp error message returned
            if (data.Code >= 200 && data.Code <= 299) {
                self.phoneGapUtils.showAlert(data.Message);
                deferred.reject();
            }
            else {
                // Upload successful so delete the record.

                var delete_promise = self.photoRecord.deleteRecord(unique_id);
                delete_promise.done(function () {
                    
                    deferred.resolve();
                });

                delete_promise.fail(function () {
                    deferred.reject();
                });

            }
        });

        // Error contacting servers (bad request?)
        upload_promise.fail(function () {
            $.mobile.loading('hide');
            self.phoneGapUtils.showAlert("There has been an error contacting the LinguaSnapp servers. Please try again later.");
            deferred.reject();
        });

        

        return deferred.promise();
    }

}