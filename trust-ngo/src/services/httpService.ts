import axiosClient from "@/api/axiosClient";

export function createResourceService<T, TCreate = Partial<T>, TUpdate = Partial<T>>(
  endpoint: string,
) {
  return {
    getAll: async (params?: Record<string, unknown>): Promise<T[]> => {
      const response = await axiosClient.get<T[]>(endpoint, { params });
      return response.data;
    },
    getById: async (id: string | number): Promise<T> => {
      const response = await axiosClient.get<T>(`${endpoint}/${id}`);
      return response.data;
    },
    create: async (payload: TCreate): Promise<T> => {
      const response = await axiosClient.post<T>(endpoint, payload);
      return response.data;
    },
    update: async (id: string | number, payload: TUpdate): Promise<T> => {
      const response = await axiosClient.put<T>(`${endpoint}/${id}`, payload);
      return response.data;
    },
    remove: async (id: string | number): Promise<void> => {
      await axiosClient.delete(`${endpoint}/${id}`);
    },
  };
}
