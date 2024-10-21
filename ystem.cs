using System;
using System.IO;
using System.Speech.Recognition;
using System.Windows.Forms;
using NAudio.Wave;


namespace RemoteMicrophone
{
    public partial class MainForm : Form
    {
        private RemoteMicrophoneModule remoteMicrophoneModule;


        public MainForm()
        {
            InitializeComponent();
            remoteMicrophoneModule = new RemoteMicrophoneModule();
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            remoteMicrophoneModule.ActivateMicrophone();
            remoteMicrophoneModule.CaptureAudio();
        }


        private void btnStop_Click(object sender, EventArgs e)
        {
            remoteMicrophoneModule.StopCapture();
        }
    }


    public class RemoteMicrophoneModule
    {
        private WaveInEvent waveSource;
        private const int CHUNK = 1024;
        private const int RATE = 44100;
        private bool isCapturing;
        private SpeechRecognitionEngine recognizer;
        private MemoryStream audioStream;


        public RemoteMicrophoneModule()
        {
            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(RATE, 1); // 44.1kHz, mono
            waveSource.DataAvailable += OnDataAvailable;


            // Inicializar el reconocimiento de voz
            recognizer = new SpeechRecognitionEngine();
            recognizer.LoadGrammar(new DictationGrammar());
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;


            // Inicializar el MemoryStream para almacenar datos de audio
            audioStream = new MemoryStream();
        }


        public void ActivateMicrophone()
        {
            isCapturing = true;
            waveSource.StartRecording();
            recognizer.SetInputToAudioStream(audioStream, new SpeechAudioFormatInfo(
                EncodingFormat.Pcm,
                RATE,
                16,
                1,
                2,
                CHUNK,
                null));
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            Console.WriteLine("Micrófono activado.");
        }


        public void CaptureAudio()
        {
            Console.WriteLine("Capturando audio...");
            while (isCapturing)
            {
                System.Threading.Thread.Sleep(100); // Simulación de procesamiento
            }
        }


        public void StopCapture()
        {
            isCapturing = false;
            waveSource.StopRecording();
            recognizer.RecognizeAsyncStop();
            Console.WriteLine("Captura de audio detenida.");
        }


        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            // Procesar los datos de audio aquí
            audioStream.Write(e.Buffer, 0, e.BytesRecorded); // Escribir en el MemoryStream


            // Guardar datos en un archivo
            using (var fileStream = new FileStream("capturedAudio.wav", FileMode.Append, FileAccess.Write))
            {
                fileStream.Write(e.Buffer, 0, e.BytesRecorded);
            }


            Console.WriteLine($"Datos de audio capturados: {e.BytesRecorded} bytes.");
        }


        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine($"Palabra clave detectada: {e.Result.Text}");
        }


        public void DetectModule()
        {
            // Implementar el código de detección aquí
            if (isCapturing)
            {
                Console.WriteLine("El módulo está activo y capturando audio.");
            }
            else
            {
                Console.WriteLine("El módulo no está activo.");
            }
        }
    }


    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
