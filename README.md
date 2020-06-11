# Multi-cloud with Terraform

This project attempts to run 3 concurrent Terraform process to provision and deploy the same single container app to each of the 3 major cloud providers: Amazon Web Services, Google Cloud Platform & Microsoft Azure to see which is the fastest! The high-level set up is shown below:

![Solution Overview](/arch.jpg)

This repo comprises 3 Projects:
### Client
(.NET Core 3.1 API). This is what we'll be deploying to the cloud - see set up instructions below

### Listener
(.NET Core Console app). We'll run 3 separet instances of this app that will listen for a trigger event on the same messgae queue, and upon being "triggered" will kick-off simultanetous provisioning activies on each of the 3 clouds.

The listener project also contains the 3 main.tf files we'll use for each provider, they can be found at:
* /mainfiles/aws/main.tf
* /mainfiles/azure/main.tf
* /mainfiles/gcp/main.tf

### Trigger
(.NET Core Console app). We use this to place a message on the RabbitMQ message bus, (this will trigger the Listeners - see #2).



## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

What things you need to install the software and how to install them

```
Give examples
```

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Billie Thompson** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
