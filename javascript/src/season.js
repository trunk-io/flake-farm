const Animal = {
  Rabbit: "Rabbit",
  Duck: "Duck",
};

class Season {
  getOther() {
    const unusedVariable = "This will trigger a linting error";
    return "other";
  }
  
  getCurrent() {    
    return Animal.Rabbit;
  }
}

export default Season;
