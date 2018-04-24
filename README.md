# Kaladin

*Kaladin is a WIP*

## How to use ?

### In development mode

1. Run: `./fake.sh build -t Watch`
2. Go to [http://localhost:8080/](http://localhost:8080/)

In development mode, we activate:

- [Hot Module Replacement](https://fable-elmish.github.io/hmr/), modify your code and see the change on the fly
- [Redux debugger](https://fable-elmish.github.io/debugger/), allow you to debug each message in your application using [Redux dev tool](https://github.com/reduxjs/redux-devtools)

### Build for production

1. Run: `./fake.sh build`
2. All the files needed for deployment are under the `output` folder.
