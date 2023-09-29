How to submit changes
=====================

Please try to follow the guidelines below. They will make things
easier on the maintainers. Not all of these guidelines matter for every
trivial change so apply some common sense.

If you are unsure about something written here, ask on the mailing list
xs-devel@lists.xenserver.org.

0.    Before starting a big project, discuss it on the list first :-)

1.    Always test your changes, however small, by both targetted 
      manual testing and by running the unit tests.

2.	  When adding new functionality, include test cases for any 
	  * important; or 
	  * difficult to manually test; or 
	  * easy to break
	  new code.

3.    All submissions must be made under the terms of the "Developer's
      Certificate of Origin" (DCO) and should include a Signed-off-by:
      line.
	  
4.    Make your patch(es) available by creating one or more github pull requests. 
      Each pull request should be separately reviewable and mergable. Only patches 
      which must be committed together should be in the same pull request.

5.    Each patch should include a descriptive commit comment that helps
      understand why the patch is necessary and why it works. This will
      be used both for initial review and for new people to understand
      how the code works later.

6.    For bonus points, ensure the project still builds in between every
      patch in a set: this helps hunt down future regressions with 'bisect'.

7.    Make sure you have the right to submit any changes you make. If you
      do changes at work you may find your employer owns the patches
      instead of you.

----------------------------------------------------------------------------
	  
For a list of maintainers, please see [MAINTAINERS](./MAINTAINERS.md) file.
