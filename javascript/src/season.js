const Animal = {
  Rabbit: "Rabbit",
  Duck: "Duck",
};

class Season {
  getCurrent() {
    const unusedVariable = "This will trigger a linting error";
    return Animal.Rabbit;
  }
}

export default Season;
