'use strict'; 

const async = require('asyncawait/async'); 
const await = require('asyncawait/await'); 
const express = require('express'); 
const bodyParser = require('body-parser');
const app = express();
const WebSocket = require('ws');
const Global = require('./util/global');

const responses = require('./util/responses'); 


function executeCall(callTitle, req, res, call) {
    exception.try(() => {
        applyCorsHeaders(res);
        var doCall = true; 

        logRequestStart(callTitle, req); 

        //reate a context object 
        const context = createCallContext(req);

        if (doCall) {
            console.log("calling " + callTitle);
            const response = await(call(context)); 
            logRequestEnd(callTitle, response);
            res.status(response.status).send(response.content);
        }
    });
}

function applyCorsHeaders(response) {
    response.setHeader('Access-Control-Allow-Origin', config.allowedOrigins);
    response.setHeader('Access-Control-Allow-Methods', 'GET,POST,PUT,DELETE');
    response.setHeader('Access-Control-Allow-Headers', 'X-Custom-Header');
    response.setHeader("Access-Control-Allow-Headers", "authtoken, Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");
}

function addOptionsCall(app, path){ 
    app.options(path, (req, res) => {
        exception.try(() => {
            logger.info('OPTIONS ' + path);

            applyCorsHeaders(res);
            res.send('{}');
        });
    });
}

function createCallContext(req) {
    return {
        query: req.query,
        body: req.body
    }
}

function logRequestStart(callTitle, request) {
    let text = callTitle; 
    let qstringParams = ""; //getRequestParamsAsString(request.query); 
    let bodyParams = null;
    if (request.body) {
        bodyParams = JSON.stringify(request.body); 
    }
    if (bodyParams === "{}")
        bodyParams = "";

    logger.info(text + " " + qstringParams + bodyParams); 
}

function logRequestEnd(callTitle, response) {
    logger.info(callTitle + " returning " + JSON.stringify(response)); 
}

const sendFile = (res, filename) => {
    res.sendfile('./public' + filename); 
};

const registerGetFile = (filename) => {
    app.get(filename, (req, res) => { 
        sendFile(res, filename);
    });
};

const addEventOptionsCalls = async(() => {
    exception.try(() => {
        const events = await(middleTier.getAllEvents()).content; 
        if (events) {
            for (let n=0; n<events.length; n++) {
                addOptionsCall(app, `/events/${events[n].id}`); 
            }
        }
    });
}); 


const wsClients = [];

function sendWs(data) {
    for (let n=0; n<wsClients.length; n++) {
        wsClients[n].send(data);
    }
}

function runWebServer (){
    const httpPort = 8085; 
    const wsPort = 8086;

    const wss = new WebSocket.Server({
        port: wsPort
    });
    console.log('mockserver wss listening on port ' + wsPort);

    wss.on('connection', function connection(ws) {
        wsClients.push(ws); 

        const sendResponse = (s) => {
            console.log('sending: ' + s); 
            ws.send(s); 
        }; 

        ws.on('message', function incoming(message) {
            console.log('msg received: ' + message); 

            const request = JSON.parse(message); 

            if (request && request.H && request.H.Type) {
                const requestType = request.H.Type; 
                let responseType = Global.RESPONSE_TYPE_NORMAL; 

                if (Global.flags.SERVER_DOWN)
                    responseType = Global.RESPONSE_TYPE_SERVERDOWN;
                if (Global.flags.AUTH_FAILURE) 
                    responseType = Global.RESPONSE_TYPE_AUTHERROR;
                if (Global.flags.RECONNECT) 
                    responseType = Global.RESPONSE_TYPE_RECONNECT;

                switch(requestType) {
                    case "System.RequestConnection": {
                        sendResponse(responses.responseConnection(request, responseType));
                        break;
                    }
                    case "Req.Devices": {
                        if (request.M && request.M.Devices && request.M.Devices.Items) 
                            sendResponse(responses.responseDeviceDetails(request, responseType)); 
                        else 
                            sendResponse(responses.responseDevices(request, responseType));                    
                        break;
                    }
                    case "Req.DeviceBriefs": {
                        sendResponse(responses.responseDeviceBriefs(request, responseType)); 
                    }
                    case "Req.Sensors": {
                        if (sendResponse(responses.responseSensors(request, 0, responseType))) {
                            setTimeout(() => {
                                sendResponse(responses.responseSensors(request, 1, responseType));
                                setTimeout(() => {
                                    sendResponse(responses.responseSensors(request, 2, responseType));
                                    setTimeout(() => {
                                        sendResponse(responses.responseSensors(request, 3, responseType));
                                        setTimeout(() => {
                                            sendResponse(responses.responseSensors(request, 4, responseType));
                                            setTimeout(() => {
                                                sendResponse(responses.responseSensors(request, 5, responseType));
                                            }, 1000); 
                                        }, 1000); 
                                    }, 1000); 
                                }, 1000); 
                            }, 1000); 
                        }
                        break;
                    }
                    case "Req.StartPivot": {
                        if (sendResponse(responses.responseStartPivot(request, 0, responseType))) {
                            setTimeout(() => {
                                sendResponse(responses.responseStartPivot(request, 1, responseType));
                                setTimeout(() => {
                                    sendResponse(responses.responseStartPivot(request, 2, responseType));
                                    setTimeout(() => {
                                        sendResponse(responses.responseStartPivot(request, 3, responseType));                                    
                                    }, 1000); 
                                }, 1000); 
                            }, 1000); 
                        }
                        break;
                    }
                    case "Req.Alerts": {
                        sendResponse(responses.responseAlerts(request, responseType)); 
                        break;
                    }
                }
            }
        });
    });


    app.use(bodyParser.json());
    app.use(bodyParser());

    addOptionsCall(app, '/');

    registerGetFile('/index.html');
    registerGetFile('/js/main.js');

    app.get('/', (req, res) => { sendFile(res, '/index.html'); });

    app.post('/sendData', (req, res) => {
        try {
            const data = req.body;

            if (data) {
                console.log('received to send: ' + JSON.stringify(data)); 
    
                sendWs(JSON.stringify(data)); 
                
                res.send('{"response":"OK"}');
            }
        }
        catch(e) {
            console.error(e);
        }
    });

    app.post('/setFlag', (req, res) => {
        try {
            const data = req.body;
            Global.flags[data.name] = data.value;
        }
        catch(e) {
            console.error(e);
        }
    });

    app.listen(httpPort, () => console.log('mockserver listening on port ' + httpPort));
}


runWebServer();