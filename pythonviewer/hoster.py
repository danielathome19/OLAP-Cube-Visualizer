import csv
import json
import matplotlib.pyplot as plt
import numpy as np
import io
import urllib, base64
import requests
import secrets
import json
from flask import Flask, render_template, request, jsonify
from requests import sessions
from bs4 import BeautifulSoup
from flask_cors import CORS
from flask.globals import session


app = Flask(__name__)
secret = secrets.token_urlsafe(32)
app.secret_key = secret
SESSION_TYPE = 'filesystem'
app.config.from_object(__name__)
CORS(app)

# data = []
# headers = []
# drillaggregates = []  # append col numbers over time
# initagg = -1
# filters = []  # col header
# filterdata = []  # col data to filter by


@app.route('/hello')
def hello():
    return 'Hello World'


@app.route('/', methods=["GET","POST"])
def index():
    if request.method == 'POST':
        if request.files.get('fileupload', False):
            f = request.files['fileupload']
            #store the file contents as a string
            fstring = f.read().decode('utf-8')

            #create list of dictionaries keyed by header row
            #csv_dicts = [{k: v for k, v in row.items()} for row in csv.DictReader(fstring.splitlines(), skipinitialspace=True)]
            headers = []
            for row in csv.DictReader(fstring.splitlines(), skipinitialspace=True):
                for k, v in row.items():
                    headers.append(k)
                break
                
            session['data'] = fstring.splitlines()  # convert back with np.ndarray()
            session['headers'] = headers
            session['drillaggregates'] = []  # append col numbers over time
            session['initagg'] = -1
            session['filters'] = []  # col header
            session['filterdata'] = []  # col data to filter by
            # session.pop('username', None) to release
            # username = session['username'] to reference
            
            print("Successfully stored session data")
            print(getDataFromSession())
    elif request.method == 'GET':
        next_url = request.args.get("message")
        print(next_url)
        print(session.get('data', None))
        return jsonify(data=session.get('data', None))

    return json.dumps({'success':True}), 200, {'ContentType':'application/json'} 


def getDataFromSession():
    firstline = True
    data = []
    for row in csv.DictReader(session['data'], skipinitialspace=True):
        temp = []
        for k, v in row.items():
            if firstline:
                data.append(v)
            else:
                temp.append(v)
        if firstline:
            firstline = False
        else:
            data = np.vstack((data, temp))
    
    return data

"""
Notes:
https://stackoverflow.com/questions/45697176/send-simple-http-request-with-html-submit-button
https://www.codeproject.com/questions/753139/pass-values-from-csharp-asp-net-array-to-javascrip
https://docs.microsoft.com/en-us/dotnet/api/system.web.script.serialization.javascriptserializer?view=netframework-4.8
https://stackoverflow.com/questions/6487167/deserializing-a-json-into-a-javascript-object
https://stackoverflow.com/questions/40963401/flask-dynamic-data-update-without-reload-page
https://flask.palletsprojects.com/en/1.1.x/quickstart/
https://stackoverflow.com/questions/10434599/get-the-data-received-in-a-flask-request
"""
app.run(host='localhost', port=5000)
