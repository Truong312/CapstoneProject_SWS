import apiClient from '@/lib/api'

export interface WhisperTranscriptionResponse {
  Text?: string
  text?: string
  Language?: string
  language?: string
  Duration?: number
  duration?: number
  Segments?: any[]
}

export interface TranscriptionResponse {
  success: boolean
  data: WhisperTranscriptionResponse
  fileName?: string
  fileSize?: number
  message?: string
}

// Single query result format
export interface SingleQueryResult {
  sql: string
  result: any[]
}

// New structured result format from backend
export interface QueryResultItem {
  queryType: string
  title: string
  totalRecords: number
  data: any[]
  summary?: {
    totalProducts?: number
    totalQuantityAvailable?: number
    totalQuantityAllocated?: number
    totalLocations?: number
    [key: string]: any
  }
  metadata: {
    source: string
    columns?: string[]
    searchKeyword?: string
  }
}

export interface OverallSummary {
  searchKeyword: string
  totalQueriesExecuted: number
  totalRecordsReturned: number
  productsFound?: number
  totalStockAvailable?: number
  totalStockAllocated?: number
}

export interface StructuredQueryResult {
  searchKeyword: string
  totalQueriesExecuted: number
  results: QueryResultItem[]
  overallSummary: OverallSummary
  originalQueries: string[]
}

// Multiple queries result format (old format - for backward compatibility)
export interface MultipleQueryResult {
  query: string
  data: any[]
  rowCount: number
  tableName?: string
  error?: string
}

export interface MultipleQueriesData {
  queries: string[]
  totalQueries: number
  results: MultipleQueryResult[]
}

// Combined result format
export interface TextToSQLResultData {
  // Single query format
  sql?: string
  result?: any[] | StructuredQueryResult
  
  // Multiple queries format (when sql contains "|||")
  queries?: string[]
  totalQueries?: number
  results?: MultipleQueryResult[]
}

export interface TextToSQLResponse {
  isSuccess: boolean
  responseCode?: string
  message: string
  data: TextToSQLResultData
}

class VoiceQueryService {
  /**
   * Upload audio file to Whisper API and get transcription
   * @param audioBlob - Audio file blob
   * @param filename - Optional filename
   * @returns Transcribed text
   */
  async transcribeAudio(audioBlob: Blob, filename: string = 'recording.webm'): Promise<{
    text: string
    language?: string
    duration?: number
  }> {
    const formData = new FormData()
    formData.append('file', audioBlob, filename)

    const response = await apiClient.post<TranscriptionResponse>(
      '/whisper/transcribe',
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    )

    const data = response.data

    // Extract transcription from response
    // Priority: data.text (lowercase from backend) > data.Text (fallback)
    const text = data?.data?.text || data?.data?.Text || ''
    
    if (!text) {
      throw new Error('No transcription found in response')
    }

    return {
      text,
      language: data?.data?.language || data?.data?.Language,
      duration: data?.data?.duration || data?.data?.Duration,
    }
  }

  /**
   * Convert natural language to SQL query and execute
   * @param naturalLanguage - Natural language query
   * @returns SQL query and results (handles both single and multiple queries)
   */
  async convertToSQL(naturalLanguage: string): Promise<{
    sql: string
    isSingleQuery: boolean
    isStructuredResult: boolean
    singleResult?: any[]
    structuredResult?: StructuredQueryResult
    multipleResults?: MultipleQueryResult[]
    totalQueries?: number
    fullResponse: TextToSQLResponse
  }> {
    const response = await apiClient.post<TextToSQLResponse>(
      '/text-to-sql/query',
      {
        naturalLanguage: naturalLanguage,
      }
    )

    const data = response.data

    // Check if this is a single query or multiple queries
    const sql = data?.data?.sql || ''
    const isSingleQuery = !sql.includes('|||')

    if (!sql) {
      throw new Error('No SQL query generated')
    }

    // Check if result is structured format (new format)
    const result = data?.data?.result
    const isStructuredResult = !!(result && typeof result === 'object' && 
                               'results' in result && 
                               'overallSummary' in result)

    if (isSingleQuery) {
      // Single query format: { sql: "...", result: [...] }
      return {
        sql,
        isSingleQuery: true,
        isStructuredResult,
        singleResult: isStructuredResult ? [] : (result as any[]) || [],
        structuredResult: isStructuredResult ? (result as StructuredQueryResult) : undefined,
        fullResponse: data,
      }
    } else {
      // Multiple queries format
      if (isStructuredResult) {
        // New structured format
        return {
          sql,
          isSingleQuery: false,
          isStructuredResult: true,
          structuredResult: result as StructuredQueryResult,
          totalQueries: (result as StructuredQueryResult).totalQueriesExecuted,
          fullResponse: data,
        }
      } else {
        // Old format: { sql: "... ||| ...", result: { queries: [...], results: [...] } }
        const multipleData = result as any
        return {
          sql,
          isSingleQuery: false,
          isStructuredResult: false,
          multipleResults: multipleData?.results || [],
          totalQueries: multipleData?.totalQueries || 0,
          fullResponse: data,
        }
      }
    }
  }

  /**
   * Full voice query pipeline: Upload audio -> Transcribe -> Convert to SQL -> Execute
   * @param audioBlob - Audio file blob
   * @returns Complete result with transcript, SQL, and data
   */
  async processVoiceQuery(audioBlob: Blob): Promise<{
    transcript: string
    language?: string
    duration?: number
    sqlQuery: string
    isSingleQuery: boolean
    isStructuredResult: boolean
    singleResult?: any[]
    structuredResult?: StructuredQueryResult
    multipleResults?: MultipleQueryResult[]
    totalQueries?: number
    fullResponse: TextToSQLResponse
  }> {
    // Step 1: Transcribe audio
    const transcription = await this.transcribeAudio(audioBlob)

    // Step 2: Convert to SQL and execute
    const sqlResult = await this.convertToSQL(transcription.text)

    return {
      transcript: transcription.text,
      language: transcription.language,
      duration: transcription.duration,
      sqlQuery: sqlResult.sql,
      isSingleQuery: sqlResult.isSingleQuery,
      isStructuredResult: sqlResult.isStructuredResult,
      singleResult: sqlResult.singleResult,
      structuredResult: sqlResult.structuredResult,
      multipleResults: sqlResult.multipleResults,
      totalQueries: sqlResult.totalQueries,
      fullResponse: sqlResult.fullResponse,
    }
  }
}

export default new VoiceQueryService()
