async function fetchApi(callback: any, url: string, method: string, body?: any) {
    let token = localStorage.Token
    let header = undefined
    if (token !== null) {
        header = new Headers({
            "Authorization": "Bearer " + token
        })
    }
    const res = await fetch(url, { method: method, body: body, headers:header });
    const data = await res.json();
    callback(data, res.status);
};
export default fetchApi