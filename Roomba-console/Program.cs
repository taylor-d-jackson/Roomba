using System;

namespace Roomba_console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var maxIters = int.Parse(args[0]);

                RunGameEngine(maxIters);
            }
            else
            {
                Console.WriteLine("Error: Please provide a single parameter representing the maximum number of iterations to run the simulation");
            }
        }

        private static void RunGameEngine(int maxIterations)
        {
            /*
             * In an infinite loop
             *   - Get input, if any
             *   - Update the state of the world
             *   - Display world to user
             */

            bool done = false;
            int iteration = 0;

            double RoombaX;
            double RoombaY;
            int RoombaAngle;

            var prng = new Random();

            InitializeWorld(out RoombaX, out RoombaY, out RoombaAngle, prng);

            var world = (RoombaX, RoombaY, RoombaAngle);
            OutputPosition(world);

            do
            {
                bool collided = false;

                iteration = GetInput(iteration);

                (world, collided) = UpdateWorld(world, prng);

                DisplayWorld(world, collided);

                done = iteration >= maxIterations;
            } while (!done);
        }

        private static void InitializeWorld(out double RoombaX, out double RoombaY, out int RoombaAngle, Random prng)
        {
            do
            {
                RoombaX = 12 * prng.NextDouble();
                RoombaY = 12 * prng.NextDouble();
                RoombaAngle = prng.Next(0, 360);
            } while (InCollision(RoombaX, RoombaY));
        }

        private static void DisplayWorld((double, double, int) world
                                        , bool collided)
        {
            if (collided)
            {
                // Output the coordinates
                OutputPosition(world);
            }
        }

        private static void OutputPosition((double, double, int) world)
        {
            var (RoombaX, RoombaY, _) = world;

            Console.WriteLine($"{RoombaX} {RoombaY}");
        }

        private static bool InCollision(double roombaX, double roombaY)
        {
            bool collision = false;

            // Out of the room?
            if (roombaX < 0 // Left of room
             || roombaX > 12 // Right of room
             || roombaY < 0 // Below room
             || roombaY > 12) // Above room
            {
                collision = true;
            }

            // In the bookcase
            else if (roombaX > 9 && roombaY < 1.5)
            {
                collision = true;
            }

            // In the left section of the sectional
            else if ((roombaX > 1 && roombaY > 5)
                  && (roombaX <= 4 && roombaY <= 8))
            {
                collision = true;
            }

            // In the top section of the sectional
            else if ((roombaX > 1 && roombaY > 8)
                  && (roombaX <= 9 && roombaY <= 11))
            {
                collision = true;
            }

            return collision;
        }

        private static int GetInput(int iteration)
        {
            return iteration + 1;
        }

        private static ((double, double, int), bool) 
            UpdateWorld((double, double, int) world
                       , Random prng)
        {
            (double, double, int) newWorld;

            var (RoombaX, RoombaY, RoombaAngle) = world;
            var RoombaAngleRadians = (RoombaAngle * Math.PI) / 180;

            var newX = RoombaX + 0.01 * Math.Sin(RoombaAngleRadians);
            var newY = RoombaY + 0.01 * Math.Cos(RoombaAngleRadians);

            var collided = InCollision(newX, newY);

            if (collided)
            {
                // Set the angle to a random angle
                var NewAngle = prng.Next(0, 360);
                newWorld = (RoombaX, RoombaY, NewAngle);
            }
            else
            {
                newWorld = (newX, newY, RoombaAngle);
            }

            return (newWorld, collided);
        }
    }
}
