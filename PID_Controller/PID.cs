using System;

namespace PID_Controller
{
    public class PID
    {
        private double kp, ki, kd;          // Controller gains
        private double n;                   // Filter coefficient
        private double Ts;                  // Sample period in seconds
        private double tsMin;               // Minimum sample period in seconds
        private double outputUpperLimit;    // Lower output limit of the controller
        private double outputLowerLimit;    // Upper output limit of the controller
        private double K;                   // Rollup parameter
        private double b0, b1, b2;          // Rollup parameters
        private double a0, a1, a2;          // Rollup parameters
        private double y0 = 0;              // Current output
        private double y1 = 0;              // Output one iteration old
        private double y2 = 0;              // Output two iterations old
        private double e0 = 0;              // Current error
        private double e1 = 0;              // Error one iteration old
        private double e2 = 0;              // Error two iterations old

        /// <summary>
        /// PID Constructor
        /// </summary>
        /// <param name="Kp">Proportional Gain</param>
        /// <param name="Ki">Integral Gain</param>
        /// <param name="Kd">Derivative Gain</param>
        /// <param name="N">Derivative FIlter Coefficient</param>
        /// <param name="OutputUpperLimit">Controller Upper Output Limit</param>
        /// <param name="OutputLowerLimit">Controller Lower Output Limit</param>
        public PID(double Kp, double Ki, double Kd, double N, double OutputUpperLimit, double OutputLowerLimit)
        {
            this.Kp = Kp;
            this.Ki = Ki;
            this.Kd = Kd;
            this.N = N;
            this.OutputUpperLimit = OutputUpperLimit;
            this.OutputLowerLimit = OutputLowerLimit;

            TsMin = 0.001;  // TsMin by default is set to 1 millisecond. 
        }

        /// <summary>
        /// PID iterator, call this function every sample period to get the current controller output.
        /// setpoint and processValue should use the same units.
        /// </summary>
        /// <param name="setPoint">Current Desired Setpoint</param>
        /// <param name="processValue">Current Process Value</param>
        /// <param name="ts">Timespan Since Last Iteration, Use Default Sample Period for First Call</param>
        /// <returns>Current Controller Outpu</returns>
        public double PID_iterate(double setPoint, double processValue, TimeSpan ts)
        {
            // Ensure the timespan is not too small or zero.
            Ts = (ts.TotalSeconds >= TsMin) ? ts.TotalSeconds : TsMin;

            // Calculate rollup parameters
            K = 2 / Ts;
            b0 = Math.Pow(K, 2) * Kp + K * Ki + Ki * N + K * Kp * N + Math.Pow(K, 2) * Kd * N;
            b1 = 2 * Ki * N - 2 * Math.Pow(K, 2) * Kp - 2 * Math.Pow(K, 2) * Kd * N;
            b2 = Math.Pow(K, 2) * Kp - K * Ki + Ki * N - K * Kp * N + Math.Pow(K, 2) * Kd * N;
            a0 = Math.Pow(K, 2) + N * K;
            a1 = -2 * Math.Pow(K, 2);
            a2 = Math.Pow(K, 2) - K * N;

            // Age errors and output history
            e2 = e1;                        // Age errors one iteration
            e1 = e0;                        // Age errors one iteration
            e0 = setPoint - processValue;   // Compute new error

            y2 = y1;                        // Age outputs one iteration
            y1 = y0;                        // Age outputs one iteration
            y0 = -a1 / a0 * y1 - a2 / a0 * y2 + b0 / a0 * e0 + b1 / a0 * e1 + b2 / a0 * e2; // Calculate current output

            // Clamp output if needed
            if (y0 > OutputUpperLimit)
            {
                y0 = OutputUpperLimit;
            }
            else if (y0 < OutputLowerLimit)
            {
                y0 = OutputLowerLimit;
            }

            return y0;
        }

        /// <summary>
        /// Reset controller history effectively resetting the controller.
        /// </summary>
        public void ResetController()
        {
            e2 = 0;
            e1 = 0;
            e0 = 0;
            y2 = 0;
            y1 = 0;
            y0 = 0;
        }

        /// <summary>
        /// Proportional Gain, consider resetting controller if this parameter is drastically changed.
        /// </summary>
        public double Kp
        {
            get { return kp; }
            set { kp = value; }
        }

        /// <summary>
        /// Integral Gain, consider resetting controller if this parameter is drastically changed.
        /// </summary>
        public double Ki
        {
            get { return ki; }
            set { ki = value; }
        }

        /// <summary>
        /// Derivative Gain, consider resetting controller if this parameter is drastically changed.
        /// </summary>
        public double Kd
        {
            get { return kd; }
            set { kd = value; }
        }

        /// <summary>
        /// Derivative filter coefficient. 
        /// A smaller N for more filtering.
        /// A larger N for less filtering. 
        /// Consider resetting controller if this parameter is drastically changed.
        /// </summary>
        public double N
        {
            get { return n; }
            set { n = value; }
        }

        /// <summary>
        /// Minimum allowed sample period to avoid dividing by zero!
        /// The Ts value can be mistakenly set to too low of a value or zero on the first iteration.
        /// </summary>
        public double TsMin
        {
            get { return tsMin; }
            set { tsMin = value; }
        }

        /// <summary>
        /// Upper output limit of the controller.
        /// This should obviously be a numerically greater value than the lower output limit.
        /// </summary>
        public double OutputUpperLimit
        {
            get { return outputUpperLimit; }
            set { outputUpperLimit = value; }
        }

        /// <summary>
        /// Lower output limit of the controller
        /// This should obviously be a numerically lesser value than the upper output limit.
        /// </summary>
        public double OutputLowerLimit
        {
            get { return outputLowerLimit; }
            set { outputLowerLimit = value; }
        }
    }
}
