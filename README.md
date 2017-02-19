# CSharp-PID
A fixed time Proportional Integral Derivative (PID) controller, written in C#.

## Controller Derivation

This class implements a Proportional-Integral-Derivative (PID) controller with a fixed sample period.
This readme assumes the reader has some understanding of a PID controller. Here I offer a bit of the logic used to derive the algorithms utilized.

We begin with the parallel form of a PID controller:

<p align="center">  <img src="/images/render.gif"/>   </p>

Hit this up with some Laplace transform magic:

<p align="center">  <img src="/images/render 1.gif"/>   </p>

While this form is great in the ideal realm, taking the derivative of an error will prove a hot mess and therefor it is much more common to modify the derivative portion with a low pass filter thusly:

<p align="center">  <img src="/images/render 2.gif"/>   </p>

Where N is the filter coefficient.

Now things get a bit interesting with some bilinear transformation. We begin by making the following substitution:  

s \leftarrow K\frac{1-z^{-1}}{1+z^{-1}}
\textup{Where}\,\, K\equiv \frac{2}{T_s}

This is where things get ugly… Making the substitution and rearranging things we now have this:

\frac{Y(z)}{E(z)}=\frac{\left(K^2\, K_{p} + K\, K_{i} + K_{i}\, N + K\, K_{p}\, N + K^2\, K_{d}\, N\right)\, z^2 + \left(2\, K_{i}\, N - 2\, K^2\, K_{p} - 2\, K^2\, K_{d}\, N\right)\, z + (K^2\, K_{p} - K\, K_{i} + K_{i}\, N - K\, K_{p}\, N + K^2\, K_{d}\, N)}{\left(K^2 + N\, K\right)\, z^2 + \left(- 2\, K^2\right)\, z + (K^2 - K\, N)}

Before proceeding this hot mess has to be made causal by having the numerator and denominator divided by the highest order of z. Leaving us with the slightly modified:

\frac{Y(z)}{E(z)}=\frac{\left(K^2\, K_{p} + K\, K_{i} + K_{i}\, N + K\, K_{p}\, N + K^2\, K_{d}\, N\right)\, + \left(2\, K_{i}\, N - 2\, K^2\, K_{p} - 2\, K^2\, K_{d}\, N\right)\, z^{-1} + (K^2\, K_{p} - K\, K_{i} + K_{i}\, N - K\, K_{p}\, N + K^2\, K_{d}\, N)\,z^{-2}}{\left(K^2 + N\, K\right)\, + \left(- 2\, K^2\right)\,z^{-1} + (K^2 - K\, N)\,z^{-2}}

For sanity's sake we make some substitutions:

\frac{Y(z)}{E(z)}=\frac{b_0\, + b_1\, z^{-1} + b_2\,z^{-2}}{a_0\, + a_1\,z^{-1} + a_2\,z^{-2}}

\textup{Where: }

b_0=\left(K^2\, K_{p} + K\, K_{i} + K_{i}\, N + K\, K_{p}\, N + K^2\, K_{d}\, N\right)

b_1=\left(2\, K_{i}\, N - 2\, K^2\, K_{p} - 2\, K^2\, K_{d}\, N\right)

b_2=(K^2\, K_{p} - K\, K_{i} + K_{i}\, N - K\, K_{p}\, N + K^2\, K_{d}\, N)

a_0=\left(K^2 + N\, K\right)

a_1=\left(- 2\, K^2\right)

a_2=(K^2 - K\, N)

Now we rearrange some terms:

( a_0+ a_1\, z^{-1} + a_2\, z^{-2})Y(z)=(b_0 + b_1\,  z^{-1} + b_2\,  z^{-2})E(z)

Then we take the inverse z-transform:

a_0\,y[n]+ a_1\,y[n-1] + a_2\,y[n-2]=b_0\,e[n] + b_1\,e[n-1] + b_2\,e[n-2]

Moving right along to solve for y[n]:

y[n]= -\frac{a_1}{a_0}\,y[n-1] -\frac{a_2}{a_0}y[n-2] + \frac{b_0}{a_0}\,e[n] + \frac{b_1}{a_0}\,e[n-1] + \frac{b_2}{a_0}\,e[n-2]\\ \\
Where:\\

b_0=\left(K^2\, K_{p} + K\, K_{i} + K_{i}\, N + K\, K_{p}\, N + K^2\, K_{d}\, N\right)

b_1=\left(2\, K_{i}\, N - 2\, K^2\, K_{p} - 2\, K^2\, K_{d}\, N\right)

b_2=(K^2\, K_{p} - K\, K_{i} + K_{i}\, N - K\, K_{p}\, N + K^2\, K_{d}\, N)

a_0=\left(K^2 + N\, K\right)

a_1=\left(- 2\, K^2\right)

a_2=(K^2 - K\, N)

K\,\equiv \frac{2}{T_s}

