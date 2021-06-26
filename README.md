# Invoice Worker README V 1.0

Basic usage of the Invoice Worker:

`InvoiceWorker --feed-url=<http_url> --invoice-dir=<directory_path>`

There is also another command line useful for local testing/debug which allows to read a JSON feed file from a local path, to use this option
the following parameter should be used instead:

`InvoiceWorker --local-json=<path_to_local_json_file> --invoice-dir=<directory_path>`

TODO/Future Improvements:

- [] PageSize could be read from a configuration file/command line instead of being hardcoded.
- [] DelayToRefetchInMs could be read from a configuration file/command line instead of being hardcoded.
- [] Invoice templating could be improved using a robust HTML template engine(ie Razor).
