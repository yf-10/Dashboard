// From "toast.js"
declare function ShowInfoToast(message: string): void;
declare function ShowErrorToast(message: string): void;

interface User {
    id: string;
    name: string;
    email: string;
}

interface ResponseJson {
    status: number;
    message: string;
    dataCount: number;
    data: any;
}

const FetchDataAsync = async <T>(url: string): Promise<T> => {
    const response = await fetch(url, {
        mode: "cors"
    });
    console.log(response);
    if (!response.ok) {
        throw new Error('Server response error');
    }
    const responseJson: T = await response.json();
    return responseJson;
}

const FetchDataWithHeaderAsync = async <T>(url: string, headers: HeadersInit): Promise<T> => {
    const response = await fetch(url, {
        mode: "cors",
        headers: headers
    });
    console.log(response);
    if (!response.ok) {
        throw new Error('Server response error');
    }
    const responseJson: T = await response.json();
    return responseJson;
}

const GetOrigin = (): string => {
    if (location.pathname.indexOf('dashboard')) {
        return location.origin + '/dashboard';
    } else {
        return location.origin;
    }
}
