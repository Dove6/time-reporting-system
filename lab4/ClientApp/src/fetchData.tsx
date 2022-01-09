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
            const isJson = response.headers.get('content-type')?.includes('application/json');
            const data = isJson ? await response.json() : null;
            if (!response.ok)
                return Promise.reject((data && data.message) || response.statusText);
            return Promise.resolve(data);
        });
}
