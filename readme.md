# Reservation API exercise
The exercise is built in .NET 6 as a WebAPI, and can be built and executed from visual studio.

## Notes
About 90 minutes in I realized my approach was more difficult than it really needed to be - I could have saved on time and complexity by populating a time slot repository as soon as a provider added a schedule; then as clients made reservations, populate a clientId field in the time slot. In keeping with the spirit of the exercise, I left it as-is.

## Other things I'd change
- implement an actual backend. The current in-memory implementation is useful only for testing.
- Separate the infrastructure model from the web API model, so that the endpoints are accepting/returning more targeted date rather than leaking the data model to the world. In addition, using a separate web API model would allow usage of HATEOAS to ease consumption of the API; it would be rather simple to follow a link embedded in the response of the free slot endpoint to create a reservation, and then follow a link embedded in the response of the reservation-create endpoint to confirm or cancel that same reservation.
- Add unit tests
- Add more XML comments
- Add logging