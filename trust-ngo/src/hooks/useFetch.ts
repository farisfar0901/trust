import { useEffect, useState } from "react";

interface UseFetchState<T> {
  data: T | null;
  isLoading: boolean;
  error: string | null;
}

export function useFetch<T>(fetcher: () => Promise<T>, deps: unknown[] = []): UseFetchState<T> {
  const [state, setState] = useState<UseFetchState<T>>({
    data: null,
    isLoading: true,
    error: null,
  });

  useEffect(() => {
    let isMounted = true;

    setState({ data: null, isLoading: true, error: null });

    fetcher()
      .then((data) => {
        if (isMounted) setState({ data, isLoading: false, error: null });
      })
      .catch((error: unknown) => {
        if (isMounted) {
          const message = error instanceof Error ? error.message : "Something went wrong";
          setState({ data: null, isLoading: false, error: message });
        }
      });

    return () => {
      isMounted = false;
    };
  }, deps);

  return state;
}
