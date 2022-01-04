// Adapted from https://gist.github.com/robbykraft/7578514
// Original .m file by Robby Kraft
// http://ssd.jpl.nasa.gov/txt/aprx_pos_planets.pdf

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitPos : MonoBehaviour
{
    // Current time (affected by speedup)
    private float time = 0f;

    // Time of a single Earth year in seconds
    private float yearTime = 180f;

    // Planet options
    public Planets planet = Planets.Earth;
    public float distanceScale = 1f;
    public float timeMultiplier = 1f;

    // Line Options
    public int lineSamples = 100;

    private static readonly float pi = Mathf.PI;
    public enum Planets
    {
        Mercury, Venus, Earth, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto
    }
    private static readonly double[] periods =
    {
        0.240846, 0.615, 1, 1.881, 11.86, 29.46, 84.01, 164.8, 248.10
    };

    private static readonly double[][] elements = 
        //                 a                e                I                 L            long.peri.       long.node.
        //                AU               rad              deg               deg              deg              deg
        {new double[]{0.38709927,      0.20563593,      7.00497902,      252.25032350,     77.45779628,     48.33076593},  //mercury
         new double[]{0.72333566,      0.00677672,      3.39467605,      181.97909950,    131.60246718,     76.67984255},  //venus
         new double[]{1.00000261,      0.01671123,     -0.00001531,      100.46457166,    102.93768193,      0.0       },  //earth moon barycenter
         new double[]{1.52371034,      0.09339410,      1.84969142,       -4.55343205,    -23.94362959,     49.55953891},  //mars
         new double[]{5.20288700,      0.04838624,      1.30439695,       34.39644051,     14.72847983,    100.47390909},  //jupiter
         new double[]{9.53667594,      0.05386179,      2.48599187,       49.95424423,     92.59887831,    113.66242448},  //saturn
        new double[]{19.18916464,      0.04725744,      0.77263783,      313.23810451,    170.95427630,     74.01692503},  //uranus
        new double[]{30.06992276,      0.00859048,      1.77004347,      -55.12002969,     44.96476227,    131.78422574},  //neptune
        new double[]{39.48211675,      0.24882730,     17.14001206,      238.92903833,    224.06891629,    110.30393684}}; //pluto

    private static readonly double[][] rates = 
        //              AU/Cy           rad/Cy           deg/Cy           deg/Cy              deg/Cy           deg/Cy
        {new double[]{0.00000037,      0.00001906,     -0.00594749,   149472.67411175,      0.16047689,     -0.12534081},  //mercury
         new double[]{0.00000390,     -0.00004107,     -0.00078890,    58517.81538729,      0.00268329,     -0.27769418},  //venus
         new double[]{0.00000562,     -0.00004392,     -0.01294668,    35999.37244981,      0.32327364,      0.0       },  //earth moon barycenter
         new double[]{0.00001847,      0.00007882,     -0.00813131,    19140.30268499,      0.44441088,     -0.29257343},  //mars
        new double[]{-0.00011607,     -0.00013253,     -0.00183714,     3034.74612775,      0.21252668,      0.20469106},  //jupiter
        new double[]{-0.00125060,     -0.00050991,      0.00193609,     1222.49362201,     -0.41897216,     -0.28867794},  //saturn
        new double[]{-0.00196176,     -0.00004397,     -0.00242939,      428.48202785,      0.40805281,      0.04240589},  //uranus
         new double[]{0.00026291,      0.00005105,      0.00035372,      218.45945325,     -0.32241464,     -0.00508664},  //neptune
        new double[]{-0.00031596,      0.00005170,      0.00004818,      145.20780515,     -0.04062942,     -0.01183482}}; //pluto

    // location of planet in the J2000 ecliptic plane, with the X-axis aligned toward the equinox
    private static Vector3 CalcPlanetPos(Planets planet, float time)
    {
        Vector3 ecliptic = new Vector3();
        double[] planet0 = elements[(int)planet];
        double[] per_century = rates[(int)planet];
        // step 1
        // compute the value of each of that planet's six elements
        double a = planet0[0] + per_century[0] * time;    // (au) semi_major_axis
        double e = planet0[1] + per_century[1] * time;    //  ( ) eccentricity
        double I = planet0[2] + per_century[2] * time;    //  (°) inclination
        double L = planet0[3] + per_century[3] * time;    //  (°) mean_longitude
        double ϖ = planet0[4] + per_century[4] * time;    //  (°) longitude_of_periapsis
        double Ω = planet0[5] + per_century[5] * time;    //  (°) longitude_of_the_ascending_node

        // step 2
        // compute the argument of perihelion, ω, and the mean anomaly, M
        double ω = ϖ - Ω;
        double M = L - ϖ;
    
        // step 3a
        // modulus the mean anomaly so that -180° ≤ M ≤ +180°
        while(M > 180) M-=360;  // in degrees
    
        // step 3b
        // obtain the eccentric anomaly, E, from the solution of Kepler's equation
        //   M = E - e*sinE
        //   where e* = 180/πe = 57.29578e
        double E = M + (e * 180d/ pi) * Math.Sin(M * pi / 180d);  // E0
        for(int i = 0; i< 5; i++){  // iterate for precision, 10^(-6) degrees is sufficient
            E = KeplersEquation(E, M, e);
        }

        // step 4
        // compute the planet's heliocentric coordinates in its orbital plane, r', with the x'-axis aligned from the focus to the perihelion
        ω = ω * pi / 180d;
        E = E * pi / 180d;
        I = I * pi / 180d;
        Ω = Ω * pi / 180d;
        double x0 = a * (Math.Cos(E) - e);
        double y0 = a * Math.Sqrt(1 - e * e) * Math.Sin(E);

        // step 5
        // compute the coordinates in the J2000 ecliptic plane, with the x-axis aligned toward the equinox:
        ecliptic.x = (float)((Math.Cos(ω) * Math.Cos(Ω) - Math.Sin(ω) * Math.Sin(Ω) * Math.Cos(I)) * x0 + (-Math.Sin(ω) * Math.Cos(Ω) - Math.Cos(ω) * Math.Sin(Ω) * Math.Cos(I)) * y0);
        ecliptic.y = (float)((Math.Cos(ω) * Math.Sin(Ω) + Math.Sin(ω) * Math.Cos(Ω) * Math.Cos(I)) * x0 + (-Math.Sin(ω) * Math.Sin(Ω) + Math.Cos(ω) * Math.Cos(Ω) * Math.Cos(I)) * y0);
        ecliptic.z = (float)((Math.Sin(ω) * Math.Sin(I)) * x0 + (Math.Cos(ω) * Math.Sin(I)) * y0);
        return ecliptic;
    }
    private static double KeplersEquation(double E, double M, double e)
    {
        double ΔM = M - (E - (e * 180d/ pi) * Math.Sin(E * pi / 180d));
        double ΔE = ΔM / (1 - e * Math.Cos(E * pi / 180d));
        return E + ΔE;
    }

    // Start is called once when the program begins
    private void Start()
    {
        // Generate the points for the orbit outlines
        LineRenderer line = GetComponent<LineRenderer>();
        if (line != null)
        {
            Vector3[] positions = new Vector3[lineSamples];
            for (int i = 0; i < lineSamples; i++)
                positions[i] = transform.parent.position + transform.rotation * CalcPlanetPos(planet, (float)periods[(int)planet] * (i / (float)lineSamples) / 100) * distanceScale * transform.lossyScale.x;
            line.positionCount = lineSamples;
            line.SetPositions(positions);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Apply speedup multiplier
        time += Time.deltaTime * timeMultiplier;

        // Calculate and set the local position of the planet
        transform.localPosition = CalcPlanetPos(planet, time / (100 * (yearTime != 0 ? yearTime : 1f))) * distanceScale;
    }
}