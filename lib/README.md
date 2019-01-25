# rpi-ws281x Native Libary
In order to get the wrapper to work, the native C library must be installed first. There are two methods to get the native library:
* [Clone and Build the Native Library](#Building-the-Native-ibrary)
* [Use the Pre-Built Library](#Using-the-Pre-Built-Library)

## Building the Native Library
The native library is available on GitHub - [rpi-ws281x](https://github.com/jgarff/rpi_ws281x). You will need to clone/download it on a Raspberry PI. The project uses a build helper called **scons** which will need to be installed on the device. Here is a basic outline:

```bash
$ sudo apt-get install build-essential git scons
$ git clone https://github.com/jgarff/rpi_ws281x.git
$ cd rpi_ws281x
$ scons
$ gcc -shared -o ws2811.so *.o
$ sudo cp ws2811.so /usr/lib
```
The project [Build](https://github.com/jgarff/rpi_ws281x#build) instructions mentions adjusting the parameters in main.c.  This file is used in the default test application that is created when you build the project and not used by the shared library.

## Using the Pre-Built Library
The pre-built library, **ws2811<span>.so</span>**, was built Raspberry PI 3B running Rasbian 9.6...

```html
$ screenfetch
    .',;:cc;,'.    .,;::c:,,.    pi@raspberrypi
   ,ooolcloooo:  'oooooccloo:    OS: Raspbian 9.6 stretch
   .looooc;;:ol  :oc;;:ooooo'    Kernel: armv7l Linux 4.14.79-v7+
     ;oooooo:      ,ooooooc.     Uptime: 2m
       .,:;'.       .;:;'.       Packages: 1624
       .... ..'''''. ....        Shell: 839
     .''.   ..'''''.  ..''.      Resolution: 1280x1024
     ..  .....    .....  ..      DE: LXDE
    .  .'''''''  .''''''.  .     WM: OpenBox
  .'' .''''''''  .'''''''. ''.   CPU: ARMv7 rev 4 (v7l) @ 1.2GHz
  '''  '''''''    .''''''  '''   GPU: Gallium 0.4 on llvmpipe (LLVM 3.9, 128 bits)
  .'    ........... ...    .'.   RAM: 115MiB / 875MiB
    ....    ''''''''.   .''.    
    '''''.  ''''''''. .'''''    
     '''''.  .'''''. .'''''.    
      ..''.     .    .''..      
            .'''''''            
             ......       
```

The libray should be placed in the **/usr/lib** folder on the Raspberry PI.