import Axios, { type AxiosRequestConfig, type AxiosError } from "axios";

export const AXIOS_INSTANCE = Axios.create({
	baseURL: import.meta.env.VITE_ARCHIVE_API_URL || "/api",
	headers: { "Content-Type": "application/json" },
});

export const customInstance = <T>(
	config: AxiosRequestConfig,
	options?: AxiosRequestConfig,
): Promise<T> => {
	return AXIOS_INSTANCE({ ...config, ...options }).then(({ data }) => data);
};

export type ErrorType<Error> = AxiosError<Error>;
export type BodyType<BodyData> = BodyData;
