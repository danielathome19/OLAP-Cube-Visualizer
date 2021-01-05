import matplotlib.pyplot as plt
import numpy as np
import io
import urllib, base64

"""
Functions: 
Aggregate (numeric col) -- sum all numbers {Ex: ["Summary"] [Sum all $ Amount US millions] [Record (row) Count]}
Drill Down (any) -- append column, perform aggregation {Ex: [Year] [Sum all $ Amount US millions] [Record (row) Count (per year)]} 
                    append column, perform aggregation {Ex: [Year] [Item/Category] [Sum all $ Amount US millions] [Record (row) Count (per item per year)]} 
                    example aggregations: year, [item] -> {Category, Sub-Category, Line Item}
Filter (aggregate) -- using current aggregates, filter by certain aggregate value {Ex: filter by year -> [] 2009, [] 2010}
View (aggregate) -- choose x and y dimensions (headers) to create graph (bar? add additional features?) 
"""

def headermenu(headers):
    print("Header options:")
    for i in range(0, len(headers)):
        print(f"\t{i}. {headers[i]}")


def getaggregatetable(data, drillaggregates=[], filters=[], filterdata=[]):
    filtertable = data
    for i in range(0, len(filters)):
        filtertable = filtertable[filtertable[:, filters[i]] == filterdata[i]]

    aggdata = filtertable[:, sorted(drillaggregates)]
    # TODO: num_rows, num_cols = aggdata.shape, add record count col, move aggregate to last col
    return aggdata


def getaggregateheaders(headers, drillaggregates=[]):
    aggheaders = []
    for i in range(1, len(drillaggregates)):
        aggheaders.append(headers[drillaggregates[i]])
    aggheaders.append(headers[drillaggregates[0]])
    # TODO: aggheaders.append('Record Count')
    return aggheaders


def displayaggregatetable(data, headers, drillaggregates=[], filters=[], filterdata=[]):
    if len(drillaggregates) == 1 and len(filters) == 0:
        num_rows, num_cols = data.shape
        curheaders = ['Summary', 'Sum of all {}'.format(headers[drillaggregates[0]]), 'Record Count']
        curtable = ['Summary', np.sum(data[:, drillaggregates[0]].astype(np.float)), num_rows]
        
        print(curheaders)
        print(curtable)
    else:
        aggheaders = getaggregateheaders(headers, drillaggregates)
        aggdata = getaggregatetable(data, drillaggregates, filters, filterdata)
        
        print(aggheaders)
        print(aggdata)


data = np.genfromtxt('data.csv', delimiter=',', dtype='str')
headers = data[0,:]
data = data[1:,:]
runprog = True  # need to turn program into a Flask web-service, each menuoption a function

drillaggregates = []  # append col numbers over time
headermenu(headers)
initagg = int(input("Enter initial aggregate column # (must be numeric values only): "))
drillaggregates.append(initagg)
filters = []  # col header
filterdata = []  # col data to filter by

while runprog:
    try:
        print("Menu options:\n\t0. Display Fact Table\n\t1. Display Aggregated Table\n\t2. Reinitialize Aggregate\n\t3. Drill-down\n\t4. Filter \
            \n\t5. View\n\t6. Quit\n\t7. Reset Drill Aggregates\n\t8. Reset Filters\n\t9. Remove Drill Aggregate\n\t10.Remove Filter")
        menuoption = int(input("Enter menu choice #: "))

        if menuoption == 0:    # Display Fact Table
            print(headers)
            print(data)
        if menuoption == 1:    # Display Aggregated Table
            displayaggregatetable(data, headers, drillaggregates, filters, filterdata)
        elif menuoption == 2:  # Reinitialize Aggregate
            headermenu(headers)
            initagg = int(input("Enter initial aggregate column # (must be numeric values only): "))
            
            if len(drillaggregates) == 0:
                drillaggregates.append(initagg)
            else:
                drillaggregates[0] = initagg
        elif menuoption == 3:  # Drill-down
            headermenu(headers)
            drillagg = int(input("Enter drill-down aggregate choice #: "))
            
            if not drillagg in drillaggregates:
                drillaggregates.append(drillagg)

            displayaggregatetable(data, headers, drillaggregates, filters, filterdata)
        elif menuoption == 4:  # Filter
            headermenu(headers)
            filterheader = int(input("Enter filter header choice #: "))
            filteroptions = list(set(data[:, filterheader]))

            print("Filter options:")
            for i in range(0, len(filteroptions)):
                print(f"\t{i}. {filteroptions[i]}")

            filterby = int(input("Enter filter option choice #: "))
            filter = filteroptions[filterby]
            if not filterheader in filters:
                filters.append(filterheader)
                filterdata.append(filter)
            
            if len(drillaggregates) > 1 or len(filters) > 0:
                displayaggregatetable(data, headers, drillaggregates, filters, filterdata)
            else:
                if len(filters) == 0 and len(drillaggregates) == 1:
                    filtertable = data[data[:, filterheader] == filter]
                    print(headers)
                    print(filtertable)  
        elif menuoption == 5:  # View
            x = []
            y = []

            if len(filters) == 0 and len(drillaggregates) == 1:
                headermenu(headers)
                xhead = int(input("Enter x header column #: "))  # 1
                yhead = int(input("Enter y header column #: "))  # 6
                x = data[:, xhead]
                y = data[:, yhead]
            else:
                print("Header options (remove drill aggregates to display more options):")
                for i in range(len(drillaggregates)):
                    print(f'{drillaggregates[i]}. {headers[drillaggregates[i]]}')

                xhead = int(input("Enter x header column #: "))  # 1
                yhead = int(input("Enter y header column #: "))  # 6
                x = getaggregatetable(data, drillaggregates, filters, filterdata)[:, sorted(drillaggregates).index(xhead)]
                y = getaggregatetable(data, drillaggregates, filters, filterdata)[:, sorted(drillaggregates).index(yhead)]
            
            print("Graph options:\n\t1. Bar\n\t2. Line\n\t3. Line with Marker\n\t4. Scatter\n\t5. Pie")
            graphtype = int(input("Enter graph choice #: "))
            x = np.sort(x)
            y = np.argsort(y)

            if graphtype == 1:      # Bar
                plt.bar(x, y)
                plt.xticks(x, x)
            elif graphtype == 2:    # Line
                plt.plot(x,y)
            elif graphtype == 3:    # Line with Marker
                plt.plot(x,y, marker='o')
            elif graphtype == 4:    # Scatter
                plt.scatter(x, y)
            elif graphtype == 5:    # Pie
                aggr_x = list(set(x))  # x must be numeric and non-negative, need to aggregate first
                plt.pie(aggr_x, labels=aggr_x)  # could use x col plus an aggregated y col                

            plt.title(f'Data from the CSV File: {headers[xhead]} and {headers[yhead]}', fontweight="bold")

            if not graphtype == 5:
                plt.xlabel(headers[xhead])
                plt.ylabel(headers[yhead])
            else:
                plt.xlabel(headers[xhead])

            plt.show()

            fig = plt.gcf()
            buf = io.BytesIO()
            fig.savefig(buf, format='png')
            buf.seek(0)
            string = base64.b64encode(buf.read())
            uri = 'data:image/png;base64,' + urllib.parse.quote(string)
            html = '<img src = "%s"/>' % uri
        elif menuoption == 6:  # Quit
            runprog = False
            break
        elif menuoption == 7:  # Reset Drill Aggregates
            drillaggregates = []
        elif menuoption == 8:  # Reset Filters
            filters = []
            filterdata = []
        elif menuoption == 9:  # Remove Drill Aggregate
            print("Current drill aggregates:")
            for i in range(0, len(drillaggregates)):
                print(f'{i}. {headers[drillaggregates[i]]}')
            
            removechoice = int(input("Enter choice # to remove: "))
            drillaggregates.pop(removechoice)  # drillaggregates = np.delete(drillaggregates, removechoice)
        elif menuoption == 10: # Remove Filter
            
            print("Current filters:")
            for i in range(0, len(filters)):
                print(f'{i}. {headers[filters[i]]}')
            
            removechoice = int(input("Enter choice # to remove: "))
            filters.pop(removechoice)  # filters = np.delete(filters, removechoice)
            filterdata.pop(removechoice) # filterdata = np.delete(filterdata, removechoice)
    except Exception as e:
        print(e)
        continue
        