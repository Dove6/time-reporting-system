# Time Reporting System (JSON-based storage)

Framework: ASP.NET Core 5.0 MVC

## Data format

Data is stored in JSON files in [storage](storage) directory.

### List of projects: [activity.json](storage/activity.json)
Object containing the list of projects ("activities"). Each project is identified by its code.
```json
{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "type": "object",
    "properties": {
        "activities": {
            "type": "array",
            "items": {
                "type": "object",
                "properties": {
                    "code": {
                        "type": "string"
                    },
                    "manager": {
                        "type": "string"
                    },
                    "name": {
                        "type": "string"
                    },
                    "budget": {
                        "type": "integer"
                    },
                    "active": {
                        "type": "boolean"
                    },
                    "subactivities": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "code": {
                                    "type": "string"
                                }
                            },
                            "required": [
                                "code"
                            ]
                        }
                    }
                },
                "required": [
                    "code",
                    "manager",
                    "name",
                    "budget",
                    "active",
                    "subactivities"
                ]
            }
        }
    },
    "required": [
        "activities"
    ]
}
```

### List of users: [users.json](storage/users.json)
Array of registered usernames.
```json
{
	"$schema": "http://json-schema.org/draft-04/schema#",
  	"type": "array",
	"items": {
		"type": "string"
	}
}
```

### Monthly report: *user*-*month*.json, eg. [kowalski-2021-11.json](storage/kowalski-2021-11.json)
Array of activity entries in month and accepted time summary per project.
```json
{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "type": "object",
    "properties": {
        "frozen": {
            "type": "boolean"
        },
        "entries": {
            "type": "array",
            "items": {
                "type": "object",
                "properties": {
                    "date": {
                        "type": "string"
                    },
                    "code": {
                        "type": "string"
                    },
                    "subcode": {
                        "type": "string"
                    },
                    "time": {
                        "type": "integer"
                    },
                    "description": {
                        "type": "string"
                    }
                },
                "required": [
                    "date",
                    "code",
                    "subcode",
                    "time",
                    "description"
                ]
            }
        },
        "accepted": {
            "type": "array",
            "items": {
                "type": "object",
                "properties": {
                    "code": {
                        "type": "string"
                    },
                    "time": {
                        "type": "integer"
                    }
                },
                "required": [
                    "code",
                    "time"
                ]
            }
        }
    },
    "required": [
        "frozen",
        "entries",
        "accepted"
    ]
}
```

## Running

ASP.NET Core 5.0 Runtime is required in order to run the application. You can get it [here](https://dotnet.microsoft.com/en-us/download/dotnet/5.0).

After entering the directory using CLI, execute the following line:
```
dotnet run
```
