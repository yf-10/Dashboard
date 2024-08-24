
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

const FetchData = async <T>(url: string): Promise<T> => {
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

const GetOrigin = (): string => {
    return location.origin;
}