'use strict'; 

function GlobalFlags() {
    this.AUTH_FAILURE = false;
    this.SERVER_DOWN = false;
    this.RECONNECT = false;
}

const globalFlags = new GlobalFlags(); 

function Server() {
    const _deviceBriefs = {  
        "Items":{  
           "PIVOT1":{  
              "$type":"RS.E.PivotDevice, RemoteScada",
              "Number":"1",
              "Features":{  
                 "RemoveList":[  
     
                 ]
              },
              "Type":"PIVOT",
              "Location":{  
                 "Longitude":"115.8503040",
                 "Latitude":"-31.8046688"
              },
              "Badges":{  
                 "Items":{  
                    "ActiveAlerts":{  
                       "Texts":[  
                          "28 Active Alerts",
                          "28 Alerts",
                          "28"
                       ],
                       "Severity":"4"
                    }
                 }
              },
              "IsUpdatingStatus":false,
              "IsFaultActive":false,
              "Status":{  
                 "Value":"Online",
                 "Severity":"1",
                 "UpdatedDateUtc":"1556206251",
                 "Visible":"true"
              },
              "Name":"Demo Pivot"
           },
           "MPS2":{  
              "$type":"RS.E.ReticDevice, RemoteScada",
              "Number":"2",
              "Features":{  
                 "RemoveList":[  
                    "IrrigationNext",
                    "IrrigationPrevious"
                 ]
              },
              "Type":"MPS",
              "Location":{  
                 "Longitude":"0.0000000",
                 "Latitude":"0.0000000"
              },
              "IsUpdatingStatus":false,
              "Status":{  
                 "Value":"Disabled",
                 "Severity":"0",
                 "UpdatedDateUtc":"1556200689",
                 "Visible":"true"
              },
              "Name":"Dummy2"
           }
        }
    }; 

    this.getDevices = () => {
``      
    }; 

    this.getDeviceBriefs = () => {
        return _deviceBriefs;
    }; 

    this.getGlobalAlerts = () => {

    }; 

    this.getDeviceAlerts = (deviceId) => {

    }; 
}

const server = new Server();

module.exports = {
    RESPONSE_TYPE_NONE: 0,
    RESPONSE_TYPE_NORMAL: 1,
    RESPONSE_TYPE_ERROR: 2, 
    RESPONSE_TYPE_AUTHERROR: 3, 
    RESPONSE_TYPE_SERVERDOWN: 4,
    RESPONSE_TYPE_RECONNECT: 5,

    flags: globalFlags,
    server: server
};

