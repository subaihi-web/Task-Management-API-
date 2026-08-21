export function getApiErrorMessage(error: unknown, fallback: string): string {
  const response = error as { error?: { message?: string; title?: string } };
  return response?.error?.message || response?.error?.title || fallback;
}
