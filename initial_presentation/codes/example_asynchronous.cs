public Task<List<Person>> GetAllAsync(CancellationToken cancellationToken)
{
    var persons = this.context.Persons.ToListAsync(cancellationToken);
    return persons;
}

public async Task PrintPersonsAsync(CancellationToken cancellationToken)
{
    var persons = await persons.GetAllAsync(cancellationToken).ConfigureAwait(false);
    foreach (var person in persons)
    {
        Console.WriteLine(person);
    }
}
