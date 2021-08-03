using System;

namespace RatingAdjustment.Services
{
    /** Service calculating a star rating accounting for the number of reviews
     * 
     */
    public class RatingAdjustmentService
    {
        const double MAX_STARS = 5.0;  // Likert scale
        const double Z = 1.96; // 95% confidence interval

        double _q;
        double _percent_positive;

        /** Percentage of positive reviews
         * 
         * In this case, that means X of 5 ==> percent positive
         * 
         * Returns: [0, 1]
         */
        void SetPercentPositive(double stars)
        {

            _percent_positive = (stars * 20.0) / 100.0;

        }

        /**
         * Calculate "Q" given the formula in the problem statement
         */
        void SetQ(double number_of_ratings)
        {

            double n = number_of_ratings;
            double p = _percent_positive;
            _q = Z * Math.Sqrt(((p * (1.0 - p)) + ((Z * Z) / (4.0 * n))) / n);
        }

        /** Adjusted lower bound
         * 
         * Lower bound of the confidence interval around the star rating.
         * 
         * Returns: a double, up to 5
         */
        public double Adjust(double stars, double number_of_ratings)
        {
            if (stars <= MAX_STARS)
            {
                SetPercentPositive(stars);
                SetQ(number_of_ratings);
                double n = number_of_ratings;
                double p = _percent_positive;
                double q = _q;
                double lbound = ((p + ((Z * Z) / (2.0 * n)) - q) / (1.0 + ((Z * Z) / n)));
                return (lbound / 20.0) * 100.0;
            }
            return 0.0; //Returns 0.0 if the stars are greater than 5.0
        }
    }
}
