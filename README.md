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

<p align="center">  <img src="/images/render 3.gif"/>   </p>
<p align="center">  <img src="/images/render 4.gif"/>   </p>

This is where things get ugly… Making the substitution and rearranging things we now have this:

<p align="center">  <img src="/images/render 5.gif"/>   </p>

Before proceeding this hot mess has to be made causal by having the numerator and denominator divided by the highest order of z. Leaving us with the slightly modified:

<p align="center">  <img src="/images/render 6.gif"/>   </p>

For sanity's sake we make some substitutions:

<p align="center">  <img src="/images/render 7.gif"/>   </p>

Now we rearrange some terms:

<p align="center">  <img src="/images/render 8.gif"/>   </p>

Then we take the inverse z-transform:

<p align="center">  <img src="/images/render 9.gif"/>   </p>

Moving right along to solve for y[n]:

<p align="center">  <img src="/images/render 10.gif"/>   </p>

This equation now defines how we can calculate the current output, *y[n]*, based on previous outputs, *y[n-k]*, the current error, *e[n]*, and previous errors, *e[n-k]*. 
