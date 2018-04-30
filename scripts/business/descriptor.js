// Class:   
//          descriptor
//          Descriptor functionality
// Constructor parameters:
//          db: The BrodySoft PhoneGap SQLlite plugin database reference or WebSQL database reference.

var Descriptor = function (db) {


    this.linguaDB = db;

    this.configDB = new ConfigDB(this.linguaDB);
    this.phoneGapUtils = new PhoneGapUtils();
    this.descriptorData = new DescriptorData(this.linguaDB);

    //-------------------------------------------------------------------------------------------------------------

    // buildDropDown()
    //
    // Build a drop down list for the passed descriptor 
    this.buildDropDown = function (descriptor_id) {
        var deferred = new $.Deferred();
        var self = this;

        var promise = this.descriptorData.getDescriptorValues(descriptor_id);

        promise.done(function (data) {
            // Successfully retrieved the descriptor information
            var i;
            var retHTML = "";
            for (i = 0; i < data.length; i++) {
                retHTML = retHTML + '<option value="' + data[i].value_code + '">' + $.i18n(data[i].value_name) + '</option>';
            }

            
            deferred.resolve(retHTML);
           
            
        });

        promise.fail(function () {
            // There was an error getting the descriptor values.
            deferred.reject();
        });

        return deferred.promise();
    }

}