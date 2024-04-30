import { Axios } from "axios";

export const axiosInstance = new Axios({
    transformResponse: [
        (data) => {
            try {
                const parsedData = JSON.parse(data.substring(6));
                return parsedData;
            } catch (e) {
                console.warn('Could not parse response', e);
                return data;
            }
        }
    ],
    transformRequest: [
        (data) => {
            if (data instanceof FormData) {
                return data;
            }
            return JSON.stringify(data);
        }
    ],
    headers: {
        "Content-Type": "application/json"
    }
});