#Tail

1. [What is Tail?](https://github.com/patriksvensson/tail#what-is-tail)
2. [Download](https://github.com/patriksvensson/tail#download)
3. [How to use](https://github.com/patriksvensson/tail#how-to-use)
4. [How to create a stream provider](https://github.com/patriksvensson/tail#how-to-create-a-stream-provider)
5. [Credits](https://github.com/patriksvensson/tail#credits)
6. [License](https://github.com/patriksvensson/tail#license)

###What is Tail?

A minimalistic, provider based WPF version of the UNIX `tail -f` command which can handle multiple kinds of streams.

![Image of the application](docs/images/tail_configuration.png)

![Image of the application](docs/images/tail_fileprovider.png)

###Download

You can find the latest release [here](https://github.com/patriksvensson/tail/releases).

###How to use
* Click the `open stream` button in the application bar.
* Select the stream provider in the new window (i.e. debug) and click `open stream`.
* If things are scrolling too fast, you can click the pause button to disable it.
* To resume automatic scrolling, press the play button.

###How to create a stream provider

See the [patriksvensson/tail-example](https://github.com/patriksvensson/tail-example) repository for an example of how to extend Tail.

###Credits
* [Caliburn.Micro](http://http://caliburnmicro.codeplex.com/)
* [Ninject](http://www.ninject.org/)

###License
Tail is made available under the MIT license. See the file LICENSE for the complete license text.  
Copyright 2013 Patrik Svensson.
