import type { ApiErrorBody } from '../types/job'

export async function parseApiError(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as ApiErrorBody
    if (body?.error?.message) {
      return body.error.message
    }
  } catch {
    // ignore malformed payloads
  }

  if (response.status === 429) {
    return 'Please try again in a moment.'
  }

  return 'Something went wrong. Please try again.'
}
