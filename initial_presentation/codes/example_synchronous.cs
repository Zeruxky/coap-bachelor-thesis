public List<Person> GetAll()
{
    var persons = this.context.Persons.ToList();
    return persons;
}

public void PrintPersons()
{
    foreach (var person in persons.GetAll())
    {
        Console.WriteLine(person);
    }
}