import { useState, useRef, useEffect } from 'react'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { useToast } from '@/hooks/use-toast'
import { 
  Mic, MicOff, Send, Loader2, Volume2, Database, FileAudio, 
  Table, MapPin, Info, CheckCircle2
} from 'lucide-react'
import voiceQueryService, { 
  MultipleQueryResult, 
  StructuredQueryResult
} from '@/services/voiceQueryService'

const VoiceQuery = () => {
  const { toast } = useToast()
  const [isRecording, setIsRecording] = useState(false)
  const [audioBlob, setAudioBlob] = useState<Blob | null>(null)
  const [audioUrl, setAudioUrl] = useState<string>('')
  const [isProcessing, setIsProcessing] = useState(false)
  const [transcript, setTranscript] = useState('')
  const [sqlQuery, setSqlQuery] = useState('')
  const [isSingleQuery, setIsSingleQuery] = useState(true)
  const [isStructuredResult, setIsStructuredResult] = useState(false)
  const [singleResult, setSingleResult] = useState<any[] | null>(null)
  const [structuredResult, setStructuredResult] = useState<StructuredQueryResult | null>(null)
  const [multipleResults, setMultipleResults] = useState<MultipleQueryResult[] | null>(null)
  const [recordingTime, setRecordingTime] = useState(0)
  
  const mediaRecorderRef = useRef<MediaRecorder | null>(null)
  const audioChunksRef = useRef<Blob[]>([])
  const timerRef = useRef<number | null>(null)

  useEffect(() => {
    return () => {
      if (timerRef.current) clearInterval(timerRef.current)
      if (audioUrl) URL.revokeObjectURL(audioUrl)
    }
  }, [audioUrl])

  const startRecording = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true })
      
      const mediaRecorder = new MediaRecorder(stream, {
        mimeType: 'audio/webm;codecs=opus'
      })
      
      mediaRecorderRef.current = mediaRecorder
      audioChunksRef.current = []

      mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          audioChunksRef.current.push(event.data)
        }
      }

      mediaRecorder.onstop = () => {
        const audioBlob = new Blob(audioChunksRef.current, { type: 'audio/webm' })
        setAudioBlob(audioBlob)
        const url = URL.createObjectURL(audioBlob)
        setAudioUrl(url)
        
        // Stop all tracks
        stream.getTracks().forEach(track => track.stop())
      }

      mediaRecorder.start()
      setIsRecording(true)
      setRecordingTime(0)
      
      // Start timer
      timerRef.current = setInterval(() => {
        setRecordingTime(prev => prev + 1)
      }, 1000)

      toast({
        title: 'Recording started',
        description: 'Speak your query clearly',
        duration: 2000,
      })
    } catch (error) {
      console.error('Error starting recording:', error)
      toast({
        variant: 'destructive',
        title: 'Microphone access denied',
        description: 'Please allow microphone access to record audio',
        duration: 4000,
      })
    }
  }

  const stopRecording = () => {
    if (mediaRecorderRef.current && isRecording) {
      mediaRecorderRef.current.stop()
      setIsRecording(false)
      
      if (timerRef.current) {
        clearInterval(timerRef.current)
        timerRef.current = null
      }

      toast({
        title: 'Recording stopped',
        description: 'You can now process your query',
        duration: 2000,
      })
    }
  }

  const processVoiceQuery = async () => {
    if (!audioBlob) {
      toast({
        variant: 'destructive',
        title: 'No audio',
        description: 'Please record audio first',
        duration: 3000,
      })
      return
    }

    setIsProcessing(true)
    setTranscript('')
    setSqlQuery('')
    setIsSingleQuery(true)
    setIsStructuredResult(false)
    setSingleResult(null)
    setStructuredResult(null)
    setMultipleResults(null)

    try {
      // Step 1: Transcribe audio
      toast({
        title: 'Processing...',
        description: 'Transcribing your voice',
        duration: 2000,
      })

      const result = await voiceQueryService.processVoiceQuery(audioBlob)
      
      setTranscript(result.transcript)

      toast({
        title: 'Transcription complete',
        description: `"${result.transcript}"`,
        duration: 3000,
      })

      // Step 2: SQL query generated
      const resultCount = result.isStructuredResult 
        ? result.structuredResult?.overallSummary?.totalRecordsReturned || 0
        : result.isSingleQuery 
          ? result.singleResult?.length || 0 
          : result.totalQueries || 0

      toast({
        title: '✅ Truy vấn thành công!',
        description: result.isStructuredResult
          ? `Tìm thấy ${result.structuredResult?.overallSummary?.productsFound || 0} sản phẩm`
          : result.isSingleQuery 
            ? `Nhận được ${resultCount} kết quả` 
            : `Thực thi ${resultCount} truy vấn`,
        duration: 3000,
      })

      setSqlQuery(result.sqlQuery)
      setIsSingleQuery(result.isSingleQuery)
      setIsStructuredResult(result.isStructuredResult)
      setSingleResult(result.singleResult || null)
      setStructuredResult(result.structuredResult || null)
      setMultipleResults(result.multipleResults || null)

    } catch (error: any) {
      console.error('Error processing voice query:', error)
      
      let errorMessage = 'Failed to process voice query'
      if (error?.response?.data?.message) {
        errorMessage = error.response.data.message
      } else if (error?.message) {
        errorMessage = error.message
      }

      toast({
        variant: 'destructive',
        title: 'Processing failed',
        description: errorMessage,
        duration: 5000,
      })
    } finally {
      setIsProcessing(false)
    }
  }

  const resetRecording = () => {
    setAudioBlob(null)
    if (audioUrl) {
      URL.revokeObjectURL(audioUrl)
      setAudioUrl('')
    }
    setTranscript('')
    setSqlQuery('')
    setIsSingleQuery(true)
    setIsStructuredResult(false)
    setSingleResult(null)
    setStructuredResult(null)
    setMultipleResults(null)
    setRecordingTime(0)
  }

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60)
    const secs = seconds % 60
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('vi-VN', { 
      style: 'currency', 
      currency: 'VND' 
    }).format(value)
  }

  const formatNumber = (value: number) => {
    return new Intl.NumberFormat('vi-VN').format(value)
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100 dark:from-gray-900 dark:to-gray-800">
      {/* Top Bar with Recording Controls */}
      <div className="sticky top-0 z-10 bg-white/95 dark:bg-gray-900/95 backdrop-blur-lg border-b border-gray-200 dark:border-gray-800 shadow-sm">
        <div className="container mx-auto px-4 md:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between gap-4">
            {/* Left: Title */}
            <div className="flex items-center gap-3">
              <div className="p-2.5 bg-gradient-to-br from-violet-600 to-purple-600 rounded-lg shadow-md">
                <Mic className="h-5 w-5 text-white" />
              </div>
              <div>
                <h1 className="text-xl md:text-2xl font-bold text-gray-900 dark:text-white">
                  Truy vấn Giọng nói
                </h1>
                <p className="text-xs md:text-sm text-gray-500 dark:text-gray-400">
                  Hỗ trợ tiếng Việt tự nhiên
                </p>
              </div>
            </div>

            {/* Right: Recording Controls */}
            <div className="flex items-center gap-3">
              {isRecording ? (
                <div className="flex items-center gap-3 px-4 py-2 bg-red-50 dark:bg-red-900/20 rounded-xl border-2 border-red-200 dark:border-red-800">
                  <div className="flex items-center gap-2">
                    <div className="relative">
                      <div className="absolute inset-0 bg-red-500 rounded-full animate-ping opacity-75"></div>
                      <div className="relative w-3 h-3 bg-red-500 rounded-full"></div>
                    </div>
                    <span className="text-sm font-semibold text-red-700 dark:text-red-300">
                      Đang ghi
                    </span>
                  </div>
                  <div className="w-px h-6 bg-red-300 dark:bg-red-700"></div>
                  <span className="text-lg font-mono font-bold text-red-600 dark:text-red-400 min-w-[60px]">
                    {formatTime(recordingTime)}
                  </span>
                  <Button
                    onClick={stopRecording}
                    size="sm"
                    variant="destructive"
                    className="gap-2"
                  >
                    <MicOff className="h-4 w-4" />
                    Dừng
                  </Button>
                </div>
              ) : audioUrl ? (
                <div className="flex items-center gap-2">
                  <div className="flex items-center gap-2 px-3 py-2 bg-violet-50 dark:bg-violet-900/20 rounded-lg border border-violet-200 dark:border-violet-800">
                    <FileAudio className="h-4 w-4 text-violet-600" />
                    <span className="text-sm text-violet-700 dark:text-violet-300">
                      {recordingTime}s
                    </span>
                  </div>
                  <Button
                    onClick={processVoiceQuery}
                    disabled={isProcessing}
                    size="sm"
                    className="bg-gradient-to-r from-violet-600 to-purple-600 hover:from-violet-700 hover:to-purple-700 gap-2"
                  >
                    {isProcessing ? (
                      <>
                        <Loader2 className="h-4 w-4 animate-spin" />
                        Xử lý...
                      </>
                    ) : (
                      <>
                        <Send className="h-4 w-4" />
                        Xử lý
                      </>
                    )}
                  </Button>
                  <Button
                    onClick={resetRecording}
                    size="sm"
                    variant="outline"
                    disabled={isProcessing}
                  >
                    Làm lại
                  </Button>
                </div>
              ) : (
                <Button
                  onClick={startRecording}
                  size="sm"
                  className="bg-gradient-to-r from-violet-600 to-purple-600 hover:from-violet-700 hover:to-purple-700 gap-2 shadow-md"
                >
                  <Mic className="h-4 w-4" />
                  Bắt đầu ghi âm
                </Button>
              )}
            </div>
          </div>

          {/* Audio Player (if exists) */}
          {audioUrl && (
            <div className="mt-3 pt-3 border-t border-gray-200 dark:border-gray-700">
              <audio 
                src={audioUrl} 
                controls 
                className="w-full max-w-md mx-auto"
                style={{ height: '36px' }}
              />
            </div>
          )}
        </div>
      </div>

      {/* Main Content Area */}
      <div className="container mx-auto px-4 md:px-6 lg:px-8 py-6 max-w-[1800px]">
        <Card className="shadow-lg border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900">
          <div className="p-6 md:p-8">
            {/* Transcript */}
            {transcript && (
              <div className="mb-4 p-4 bg-blue-50 dark:bg-blue-950/20 rounded-lg border border-blue-200 dark:border-blue-800">
                <div className="flex items-center gap-2 mb-2">
                  <Volume2 className="h-4 w-4 text-blue-600" />
                  <span className="text-sm font-semibold text-blue-900 dark:text-blue-300">
                    Phiên âm
                  </span>
                </div>
                <p className="text-sm text-blue-800 dark:text-blue-200">
                  "{transcript}"
                </p>
              </div>
            )}

            {/* SQL Query */}
            {sqlQuery && (
              <div className="mb-4 p-4 bg-green-50 dark:bg-green-950/20 rounded-lg border border-green-200 dark:border-green-800">
                <div className="flex items-center gap-2 mb-2">
                  <Database className="h-4 w-4 text-green-600" />
                  <span className="text-sm font-semibold text-green-900 dark:text-green-300">
                    Câu truy vấn SQL
                  </span>
                </div>
                <pre className="text-xs text-green-800 dark:text-green-200 font-mono bg-green-100 dark:bg-green-900/30 p-3 rounded overflow-x-auto whitespace-pre-wrap break-all">
                  {sqlQuery.split('|||').map((query, idx) => (
                    <div key={idx} className={idx > 0 ? 'mt-3 pt-3 border-t border-green-300 dark:border-green-700' : ''}>
                      {query.trim()}
                    </div>
                  ))}
                </pre>
              </div>
            )}

            {/* Structured Results - New Format */}
            {isStructuredResult && structuredResult && (
              <div className="space-y-4">
                {/* Overall Summary */}
                {structuredResult.overallSummary && (
                  <div className="p-4 bg-gradient-to-r from-violet-50 to-purple-50 dark:from-violet-950/20 dark:to-purple-950/20 rounded-lg border border-violet-200 dark:border-violet-800">
                    <div className="flex items-center gap-2 mb-3">
                      <CheckCircle2 className="h-5 w-5 text-violet-600" />
                      <span className="font-semibold text-violet-900 dark:text-violet-300">
                        Tổng quan kết quả
                      </span>
                    </div>
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                      {structuredResult.overallSummary.productsFound !== undefined && (
                        <div className="p-3 bg-white dark:bg-gray-800 rounded-lg">
                          <div className="text-xs text-gray-600 dark:text-gray-400">Sản phẩm</div>
                          <div className="text-xl font-bold text-violet-600">
                            {formatNumber(structuredResult.overallSummary.productsFound)}
                          </div>
                        </div>
                      )}
                      {structuredResult.overallSummary.totalStockAvailable !== undefined && (
                        <div className="p-3 bg-white dark:bg-gray-800 rounded-lg">
                          <div className="text-xs text-gray-600 dark:text-gray-400">Tồn kho khả dụng</div>
                          <div className="text-xl font-bold text-green-600">
                            {formatNumber(structuredResult.overallSummary.totalStockAvailable)}
                          </div>
                        </div>
                      )}
                      {structuredResult.overallSummary.totalStockAllocated !== undefined && (
                        <div className="p-3 bg-white dark:bg-gray-800 rounded-lg">
                          <div className="text-xs text-gray-600 dark:text-gray-400">Đã phân bổ</div>
                          <div className="text-xl font-bold text-orange-600">
                            {formatNumber(structuredResult.overallSummary.totalStockAllocated)}
                          </div>
                        </div>
                      )}
                      <div className="p-3 bg-white dark:bg-gray-800 rounded-lg">
                        <div className="text-xs text-gray-600 dark:text-gray-400">Truy vấn</div>
                        <div className="text-xl font-bold text-blue-600">
                          {structuredResult.overallSummary.totalQueriesExecuted}
                        </div>
                      </div>
                    </div>
                  </div>
                )}

                {/* Query Results */}
                {structuredResult.results.map((resultItem, index) => (
                  <div 
                    key={index}
                    className="p-4 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm"
                  >
                    {/* Header */}
                    <div className="flex items-center justify-between mb-3 pb-3 border-b border-gray-200 dark:border-gray-700">
                      <div className="flex items-center gap-2">
                        <Info className="h-4 w-4 text-violet-600" />
                        <span className="font-semibold text-gray-900 dark:text-gray-100">
                          {resultItem.title}
                        </span>
                      </div>
                      <span className="text-xs px-2 py-1 bg-violet-100 dark:bg-violet-900/30 text-violet-700 dark:text-violet-300 rounded-full">
                        {resultItem.totalRecords} kết quả
                      </span>
                    </div>

                    {/* Data - Product Details */}
                    {resultItem.queryType === 'product_details' && (
                      <div className="space-y-2">
                        {resultItem.data.map((product: any, idx: number) => (
                          <div key={idx} className="p-3 bg-gray-50 dark:bg-gray-700/50 rounded-lg">
                            <div className="font-semibold text-violet-600 mb-2">{product.Name}</div>
                            <div className="grid grid-cols-2 gap-2 text-sm">
                              <div>
                                <span className="text-gray-600 dark:text-gray-400">Mã SP:</span>
                                <span className="ml-2 text-gray-900 dark:text-gray-100">{product.SerialNumber}</span>
                              </div>
                              <div>
                                <span className="text-gray-600 dark:text-gray-400">Đơn vị:</span>
                                <span className="ml-2 text-gray-900 dark:text-gray-100">{product.Unit}</span>
                              </div>
                              <div>
                                <span className="text-gray-600 dark:text-gray-400">Giá:</span>
                                <span className="ml-2 text-gray-900 dark:text-gray-100 font-semibold">
                                  {formatCurrency(product.UnitPrice)}
                                </span>
                              </div>
                              {product.ExpiredDate && (
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">HSD:</span>
                                  <span className="ml-2 text-gray-900 dark:text-gray-100">
                                    {new Date(product.ExpiredDate).toLocaleDateString('vi-VN')}
                                  </span>
                                </div>
                              )}
                            </div>
                            {product.Description && (
                              <p className="mt-2 text-xs text-gray-600 dark:text-gray-400">{product.Description}</p>
                            )}
                          </div>
                        ))}
                      </div>
                    )}

                    {/* Data - Inventory & Location */}
                    {resultItem.queryType === 'inventory_location' && (
                      <div className="space-y-3">
                        {resultItem.data.map((item: any, idx: number) => (
                          <div key={idx} className="border border-gray-200 dark:border-gray-600 rounded-lg overflow-hidden">
                            <div className="p-3 bg-violet-50 dark:bg-violet-900/20">
                              <div className="font-semibold text-violet-900 dark:text-violet-200">
                                {item.productName}
                              </div>
                              <div className="flex gap-4 mt-2 text-sm">
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">Khả dụng:</span>
                                  <span className="ml-2 text-green-600 font-semibold">{formatNumber(item.totalAvailable)}</span>
                                </div>
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">Đã phân bổ:</span>
                                  <span className="ml-2 text-orange-600 font-semibold">{formatNumber(item.totalAllocated)}</span>
                                </div>
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">Tổng:</span>
                                  <span className="ml-2 text-blue-600 font-semibold">{formatNumber(item.totalInStock)}</span>
                                </div>
                              </div>
                            </div>
                            {item.locations && item.locations.length > 0 && (
                              <div className="p-3">
                                <div className="text-xs font-semibold text-gray-700 dark:text-gray-300 mb-2">
                                  Vị trí kho:
                                </div>
                                <div className="grid grid-cols-2 md:grid-cols-3 gap-2">
                                  {item.locations.map((loc: any, locIdx: number) => (
                                    <div key={locIdx} className="flex items-center gap-2 text-xs p-2 bg-gray-100 dark:bg-gray-700 rounded">
                                      <MapPin className="h-3 w-3 text-violet-600" />
                                      <span className="text-gray-900 dark:text-gray-100">
                                        Kệ {loc.shelf} - Cột {loc.column} - Hàng {loc.row}
                                      </span>
                                      <span className="ml-auto text-violet-600 font-semibold">
                                        {formatNumber(loc.quantity)}
                                      </span>
                                    </div>
                                  ))}
                                </div>
                              </div>
                            )}
                          </div>
                        ))}

                        {/* Summary */}
                        {resultItem.summary && (
                          <div className="mt-3 p-3 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
                            <div className="text-xs font-semibold text-blue-900 dark:text-blue-300 mb-2">Tổng kết:</div>
                            <div className="grid grid-cols-2 md:grid-cols-4 gap-2 text-xs">
                              {resultItem.summary.totalProducts !== undefined && (
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">Sản phẩm:</span>
                                  <span className="ml-2 font-semibold">{resultItem.summary.totalProducts}</span>
                                </div>
                              )}
                              {resultItem.summary.totalQuantityAvailable !== undefined && (
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">Khả dụng:</span>
                                  <span className="ml-2 font-semibold">{formatNumber(resultItem.summary.totalQuantityAvailable)}</span>
                                </div>
                              )}
                              {resultItem.summary.totalQuantityAllocated !== undefined && (
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">Phân bổ:</span>
                                  <span className="ml-2 font-semibold">{formatNumber(resultItem.summary.totalQuantityAllocated)}</span>
                                </div>
                              )}
                              {resultItem.summary.totalLocations !== undefined && (
                                <div>
                                  <span className="text-gray-600 dark:text-gray-400">Vị trí:</span>
                                  <span className="ml-2 font-semibold">{resultItem.summary.totalLocations}</span>
                                </div>
                              )}
                            </div>
                          </div>
                        )}
                      </div>
                    )}

                    {/* Fallback - Generic Data Display */}
                    {!['product_details', 'inventory_location'].includes(resultItem.queryType) && (
                      <div className="max-h-64 overflow-auto">
                        <pre className="text-xs text-gray-800 dark:text-gray-200 font-mono bg-gray-100 dark:bg-gray-700 p-3 rounded">
                          {JSON.stringify(resultItem.data, null, 2)}
                        </pre>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}

            {/* Query Results - Single Query (Old Format) */}
            {!isStructuredResult && isSingleQuery && singleResult && singleResult.length > 0 && (
              <div className="p-4 bg-violet-50 dark:bg-violet-950/20 rounded-lg border border-violet-200 dark:border-violet-800">
                <div className="flex items-center gap-2 mb-2">
                  <Database className="h-4 w-4 text-violet-600" />
                  <span className="text-sm font-semibold text-violet-900 dark:text-violet-300">
                    Kết quả truy vấn ({singleResult.length} dòng)
                  </span>
                </div>
                <div className="max-h-96 overflow-auto">
                  <pre className="text-xs text-violet-800 dark:text-violet-200 font-mono bg-violet-100 dark:bg-violet-900/30 p-3 rounded">
                    {JSON.stringify(singleResult, null, 2)}
                  </pre>
                </div>
              </div>
            )}

            {/* Query Results - Multiple Queries (Old Format) */}
            {!isStructuredResult && !isSingleQuery && multipleResults && multipleResults.length > 0 && (
              <div className="space-y-4">
                <div className="p-3 bg-blue-50 dark:bg-blue-950/20 rounded-lg border border-blue-200 dark:border-blue-800">
                  <div className="flex items-center gap-2">
                    <Database className="h-4 w-4 text-blue-600" />
                    <span className="text-sm font-semibold text-blue-900 dark:text-blue-300">
                      Tìm thấy {multipleResults.length} bảng dữ liệu
                    </span>
                  </div>
                </div>

                {multipleResults.map((queryResult, index) => (
                  <div 
                    key={index}
                    className="p-4 bg-violet-50 dark:bg-violet-950/20 rounded-lg border border-violet-200 dark:border-violet-800"
                  >
                    <div className="mb-3">
                      <div className="flex items-center gap-2 mb-1">
                        <Table className="h-4 w-4 text-violet-600" />
                        <span className="text-sm font-semibold text-violet-900 dark:text-violet-300">
                          {queryResult.tableName || `Kết quả ${index + 1}`}
                        </span>
                        <span className="text-xs text-violet-600 dark:text-violet-400">
                          ({queryResult.rowCount} dòng)
                        </span>
                      </div>
                      <pre className="text-xs text-gray-600 dark:text-gray-400 font-mono bg-gray-100 dark:bg-gray-800 p-2 rounded overflow-x-auto">
                        {queryResult.query}
                      </pre>
                    </div>
                    
                    {queryResult.error ? (
                      <div className="p-3 bg-red-100 dark:bg-red-900/30 rounded border border-red-300 dark:border-red-700">
                        <p className="text-xs text-red-800 dark:text-red-200">
                          Lỗi: {queryResult.error}
                        </p>
                      </div>
                    ) : (
                      <div className="max-h-64 overflow-auto">
                        <pre className="text-xs text-violet-800 dark:text-violet-200 font-mono bg-violet-100 dark:bg-violet-900/30 p-3 rounded">
                          {JSON.stringify(queryResult.data, null, 2)}
                        </pre>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}

            {/* Empty State */}
            {!transcript && !sqlQuery && (
              <div className="text-center py-12 lg:py-20 text-gray-400">
                <div className="relative inline-block mb-6">
                  <div className="absolute inset-0 bg-violet-400 rounded-full blur-2xl opacity-20 animate-pulse"></div>
                  <Database className="h-16 w-16 lg:h-20 lg:w-20 mx-auto relative opacity-50" />
                </div>
                <p className="text-sm md:text-base font-medium">
                  Ghi âm và xử lý truy vấn của bạn để xem kết quả
                </p>
                <p className="text-xs md:text-sm mt-2 text-gray-400">
                  Hệ thống hỗ trợ tiếng Việt tự nhiên
                </p>
              </div>
            )}
          </div>
        </Card>

        {/* Instructions Card */}
        <Card className="mt-6 p-6 shadow-md border border-gray-200 dark:border-gray-700 bg-gradient-to-br from-violet-50 to-purple-50 dark:from-violet-950/20 dark:to-purple-950/20">
          <div className="flex items-start gap-3">
            <div className="p-2 bg-violet-600 rounded-lg flex-shrink-0">
              <Info className="h-5 w-5 text-white" />
            </div>
            <div className="flex-1">
              <h3 className="font-semibold mb-4 text-violet-900 dark:text-violet-300 text-lg">
                Hướng dẫn sử dụng
              </h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="flex gap-3 items-start">
                  <div className="flex-shrink-0 w-7 h-7 rounded-full bg-violet-600 text-white text-sm font-bold flex items-center justify-center">
                    1
                  </div>
                  <p className="text-sm text-gray-700 dark:text-gray-300">
                    Nhấn <span className="font-semibold text-violet-600">"Bắt đầu ghi âm"</span> và nói rõ câu hỏi
                  </p>
                </div>
                <div className="flex gap-3 items-start">
                  <div className="flex-shrink-0 w-7 h-7 rounded-full bg-violet-600 text-white text-sm font-bold flex items-center justify-center">
                    2
                  </div>
                  <p className="text-sm text-gray-700 dark:text-gray-300">
                    Nhấn <span className="font-semibold text-red-600">"Dừng"</span> khi hoàn tất
                  </p>
                </div>
                <div className="flex gap-3 items-start">
                  <div className="flex-shrink-0 w-7 h-7 rounded-full bg-violet-600 text-white text-sm font-bold flex items-center justify-center">
                    3
                  </div>
                  <p className="text-sm text-gray-700 dark:text-gray-300">
                    Kiểm tra và nhấn <span className="font-semibold text-purple-600">"Xử lý"</span> để truy vấn
                  </p>
                </div>
                <div className="flex gap-3 items-start">
                  <div className="flex-shrink-0 w-7 h-7 rounded-full bg-violet-600 text-white text-sm font-bold flex items-center justify-center">
                    4
                  </div>
                  <p className="text-sm text-gray-700 dark:text-gray-300">
                    Xem kết quả với thông tin sản phẩm, tồn kho và vị trí
                  </p>
                </div>
              </div>
            </div>
          </div>
        </Card>
      </div>
    </div>
  )
}

export default VoiceQuery
