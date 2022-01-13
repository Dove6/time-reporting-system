export default function fetchData(url: string, method: string = 'GET', payload?: {}) {
    const config: RequestInit = {
        method: method
    };
    if (payload !== undefined) {
        config.headers = {
            'Content-Type': 'application/json'
        };
        config.body = JSON.stringify(payload);
    }
    return fetch(url, config)
        .then(async response => {
            // based on: https://jasonwatmore.com/post/2021/09/22/fetch-vanilla-js-check-if-http-response-is-json-in-javascript
            const isJson = response.headers.get('content-type')?.includes('application/json') ||
                response.headers.get('content-type')?.includes('application/problem+json');
            const data = isJson ? await response.json() : null;
            if (!response.ok) {
                console.log(data);
                let returnData = response.statusText;
                if (data && data.message)
                    returnData = data.message;
                if (data && data.errors && Object.entries(data.errors)[0])
                    returnData = `${Object.entries(data.errors)[0][0]}: ${Object.entries(data.errors)[0][1]}`
                return Promise.reject(returnData);
            }
            return Promise.resolve(data);
        });
}
