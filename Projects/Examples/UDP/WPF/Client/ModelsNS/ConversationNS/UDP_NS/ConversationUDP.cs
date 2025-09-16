using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.ModelsNS.ConversationNS.UDP_NS
{
    internal class ConversationUDP
    {
        #region Таймер опроса контроллера.

        #region Свойства.

        private static int continuousQueryCount;

        public static int ContinuousQueryCount
        {
            get { return continuousQueryCount; }
            set { continuousQueryCount = value; }
        }

        #endregion Свойства.

        #region Методы.

        /// <summary>
        /// Запустить таймер опроса контроллера.
        /// </summary>
        void LaunchTimerServerMessage()
        {
            // Делегат для типа Timer.
            timerServerMessageCallback = new TimerCallback(TimerServerMessageCallback);            

            /// Запустить таймер опроса контроллера.
            timerServerMessage = new Timer(timerServerMessageCallback, null, 0, periodTimerServerMessage);
        }

        static Timer timerServerMessage;            // Таймер сообщений сервера.
        const int periodTimerServerMessage = 50;    // Период таймера сообщений сервера.                                                        
        TimerCallback timerServerMessageCallback;   // Делегат для типа Timer.        

        // Функция обратного вызова для таймера сообщений сервера.
        void TimerServerMessageCallback(object state)
        {                    
            ReadBuffer();
        }
        
        /// Запрос в контроллер.     
        private void ReadBuffer()
        {

            /*ChangePeriodOfTimerRequestController();

            byte[] queryBytes; // Байты запроса.

            #region Плата №1.

            #region Угол.            

            if (AngleGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                AngleGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                /// 
                ///   _____________TimerRequestControllerTick_______________           timerRequestController
                ///  |                                                      |                    |
                ///  время на (WriteBuffer() + Thread.Sleep() + ReadBuffer())        <         period
                ///  
                queryBytes = Angle.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion Угол.

            #region Время на угле.            

            if (AngleTimeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                AngleTimeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                queryBytes = AngleTime.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion Время на угле.

            #region Энкодер.

            if (EncoderGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                EncoderGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                queryBytes = Encoder.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion Энкодер.

            #region Строб.           

            if (StrobeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                StrobeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                queryBytes = Strobe.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion Строб.

            #region Инфо.

            if (InfoBoardGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                InfoBoardGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                queryBytes = InfoBoard.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion Инфо.

            #endregion Плата №1.

            #region Плата №2.

            #region Инфо.

            if (InfoBoard1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On)
            {
                queryBytes = InfoBoard1.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion Инфо.

            #region СтартСтоп. Усреднение.           

            // Канал №1.
            if (AverageGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                AverageGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                AverageGroupUC.UpdateClassQueryProperties();
                queryBytes = Average.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            // Канал №2.
            if (Average1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                Average1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                Average1GroupUC.UpdateClassQueryProperties();
                queryBytes = Average1.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion Усреднение.

            #region СтартСтоп. ПериодСдвиг.           

            // Канал №1.
            if (PeriodShiftGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                PeriodShiftGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                PeriodShiftGroupUC.UpdateClassQueryProperties();
                queryBytes = PeriodShift.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            // Канал №2.
            if (PeriodShift1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                PeriodShift1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                PeriodShift1GroupUC.UpdateClassQueryProperties();
                queryBytes = PeriodShift1.QueryBytes;
                Conversation.QueryInController(queryBytes);
                Thread.Sleep(timeForInsertCommand);
            }

            #endregion СтартСтоп. ПериодСдвиг.

            #endregion Плата №2.*/

        }

        /*int previousContinuouseQueryCount;

        /// <summary>
        /// Минимальный период опроса контроллера.
        /// Когда опрашивается один параметр.
        /// К примеру Angle.AnswerQuery. В это
        /// время входит 
        /// 1. Задержка на запрос в конроллер и 
        /// получение ответа от него: 10 мс
        /// по RS-485.
        /// 2. Окно для вклинивания команды: 40 мс.
        /// По RS-485.
        /// </summary>        
        const int minPeriodTimerRequestController = 50;

        /// <summary>
        /// Временное окно для отправки команды в контроллер.
        /// Запросы идут непрерывно и чтобы вклиниться 
        /// с командой нужно временное окно. Оно должно
        /// быть больше времени на отправку команды в 
        /// контроллер и приём ответа от контроллера.
        /// </summary>
        int timeForInsertCommand = 4 * Conversation.waitingTime;

        /// <summary>
        /// Время ожидания ответа от контроллера.
        /// Плюс время на чтение ответа от 
        /// контроллера.
        /// </summary>
        int waitingTime = Conversation.waitingTime;

        /// <summary>
        /// Изменить период таймера запросов пропорционально количеству запросов.
        /// </summary>
        void ChangePeriodOfTimerRequestController()
        {
            continuousQueryCount = GetContinuousQueryCount();
            int periodTimerRequestController;
            if (continuousQueryCount != previousContinuouseQueryCount)
            {
                ///periodTimerRequestController = waitingTime * (continuousQueryCount - 1) + timeForInsertCommand;
                periodTimerRequestController = continuousQueryCount * (waitingTime + timeForInsertCommand);
                if (timerRequestController != null)
                {
                    if (periodTimerRequestController > minPeriodTimerRequestController)
                    {
                        _ = timerRequestController.Change(0, periodTimerRequestController);
                    }
                    else
                    {
                        _ = timerRequestController.Change(0, minPeriodTimerRequestController);
                    }
                }
            }

            previousContinuouseQueryCount = continuousQueryCount;
        }*/

        /*public int GetContinuousQueryCount()
        {
            #region Параметры таймера.

            // Параметр для изменения периода таймера.
            int continuousQueryCount = 0;

            #endregion Параметры таймера.

            #region Плата №1.

            #region Инфо.

            if (InfoBoardGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                InfoBoardGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion Инфо.

            #region Угол.            

            if (AngleGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                AngleGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion Угол.            

            #region Энкодер.                  

            if (EncoderGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On)
            {
                continuousQueryCount++;
            }

            #endregion Энкодер.            

            #region Тормоз.                       

            if (BrakeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                BrakeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion Тормоз.

            #region Стоп.


            #endregion Стоп.           

            #region Строб.

            if (StrobeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                StrobeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion Строб.            

            #region Строб. Параметры.

            if (StrobeParamGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                StrobeParamGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion Строб. Параметры. 

            #region Время на угле.            

            if (AngleTimeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                AngleTimeGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion Время на угле.

            #endregion Плата №1.

            #region Плата №2.

            #region Инфо.

            if (InfoBoard1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                InfoBoard1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion Инфо.

            #region СтартСтоп. Усреднение.

            if (AverageGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                AverageGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion СтартСтоп. Усреднение.

            #region СтартСтоп. ПериодСдвиг.

            if (PeriodShiftGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                PeriodShiftGroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion СтартСтоп. ПериодСдвиг.

            #region СтартСтоп. ПериодСдвиг1.

            if (PeriodShift1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.On ||
                PeriodShift1GroupUC.CheckBoxStateEnum == CheckBoxStateEnum.NeedOn)
            {
                continuousQueryCount++;
            }

            #endregion СтартСтоп. ПериодСдвиг1.

            #endregion Плата №2.            

            return continuousQueryCount;
        }*/

        // Счётчик итраций.
        public byte iTimerRequestControllerEvent;        

        internal static void StopTimerServerMessage()
        {
            // Остановить таймер опроса контроллера.
            if (timerServerMessage != null)
            {
                timerServerMessage.Dispose();
            }

        }

        #endregion Методы.

        #endregion Таймер опроса контроллера.                             
    }
}
