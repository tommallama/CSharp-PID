# CSharp-PID
A Proportional Integral Derivative (PID) controller, written in C#.
Use this project as a guide for implementing a PID controller.

## Limitation 
Implementing anti-windup in the final discrete form would prove quite the effort.
Consider another form if you need an aggressive integrator and/or windup will be an issue.

## Constructor
```cs
public PID(double Kp, double Ki, double Kd, double N, double OutputUpperLimit, double OutputLowerLimit)
```
* ```Kp``` Proportional Gain
* ```Ki``` Integral Gain
* ```Kd``` Derivative Gain
* ```N``` Derivative FIlter Coefficient
* ```OutputUpperLimit``` Controller Upper Output Limit
* ```OutputLowerLimit``` Controller Lower Output Limit

## Methods
```cs
public double PID_iterate(double setPoint, double processValue, TimeSpan ts)
```
 The **PID_iterate** method is called once a sample period to run the controller.
* **setPoint** is the desired (target) setpoint for the system to reach. 
* **processValue** is the current process value which is being controlled.
* **ts** is the TimeSpan since the last time **PID_iterate** was called.
* The returned value is the controller output (input to what is being controlled).

```cs
public void ResetController()
```
**ResetController** resets the controller history effectively resetting the controller.
* This should be called when the system is idle if gains have been drastically modified.

## Properties
| Property            | Type        | Access  | Description                                 |
|:-------------------:|:-----------:|:-------:|---------------------------------------------|
| Kd                  | ```double``` | get/set | The proportional gain                     	|
| Ki                  | ```double``` | get/set | The integral gain                        	|
| Kd                  | ```double``` | get/set | The derivative gain                       	|
| N                   | ```double``` | get/set | The derivative filter coefficient 		    	|
| TsMin               | ```double``` | get/set | The minimum sample time allowed 		      	|
| OutputUpperLimit    | ```double``` | get/set | The maximum value the controller return    |
| OutputLowerLimit    | ```double``` | get/set | The minimum value the controller return	  |

### Notes on Properties
* The **N** property is the filter coefficient on the derivative terms low pass filter. A higher value of **N** equates to less filtering and a lower value of **N** equates to more filtering. See the **Controller Derivation** section below for technical details.
* The **TsMin** property is by default **1 millisecond**. **TsMin** should be left alone so long the intended sample period is greater than 1 millisecond. This property mostly exists to keep the controller from being passed a zero timespan and causing a division by zero.

## Controller Derivation
This class implements a Proportional-Integral-Derivative (PID) controller with a fixed sample period.
This readme assumes the reader has some understanding of a PID controller. Here I offer a bit of the logic used to derive the algorithms utilized.

We begin with the parallel form of a PID controller:

<p align="center">  <img src="/assets/render.gif"/>   </p>

Hit this up with some Laplace transform magic:

<p align="center">  <img src="/assets/render 1.gif"/>   </p>

While this form is great in the ideal realm, taking the derivative of an error will prove a hot mess and therefore it is much more common to modify the derivative portion with a low pass filter thusly:

<p align="center">  <img src="/assets/render 2.gif"/>   </p>

Where N is the filter coefficient.

Now things get a bit interesting with some bilinear transformation. We begin by making the following substitution:  

<p align="center">  <img src="/assets/render 3.gif"/>   </p>
<p align="center">  <img src="/assets/render 4.gif"/>   </p>

This is where things get ugly… Making the substitution and rearranging things we now have this:

<p align="center">  <img src="/assets/render 5.gif"/>   </p>

Before proceeding, this hot mess has to be made causal by having the numerator and denominator divided by the highest order of z. Leaving us with the slightly modified:

<p align="center">  <img src="/assets/render 6.gif"/>   </p>

For sanity's sake we make some substitutions:

<p align="center">  <img src="/assets/render 7.gif"/>   </p>

Now we rearrange some terms:

<p align="center">  <img src="/assets/render 8.gif"/>   </p>

Then we take the inverse z-transform:

<p align="center">  <img src="/assets/render 9.gif"/>   </p>

Moving right along to solve for y[n]:

<p align="center">  <img src="/assets/render 10.gif"/>   </p>

This equation now defines how we can calculate the current output, *__y[n]__*, based on previous outputs, *__y[n-k]__*, the current error, *__e[n]__*, and previous errors, *__e[n-k]__*. 
