using System.Security.Principal;
using FubuMVC.Authentication.Membership;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Http;
using FubuTestingSupport;
using NUnit.Framework;

using Rhino.Mocks;

namespace FubuMVC.Authentication.Tests
{
    [TestFixture]
    public class when_authenticating_and_there_is_not_a_previous_authentication_token : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;
        private FubuContinuation theRedirect;

        protected override void beforeEach()
        {
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);

            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(false);

            theRedirect = FubuContinuation.RedirectTo<LoginRequest>();

            MockFor<IAuthenticationRedirector>().Stub(x => x.Redirect())
                                                .Return(theRedirect);

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_redirect_based_on_what_IAuthenticationRedirector_decides()
        {
            theResult.ShouldBeTheSameAs(theRedirect);
        }
    }

    [TestFixture]
    public class when_authenticating_and_there_is_a_previously_authenticated_user : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;

        protected override void beforeEach()
        {
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(false);
            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(true);

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_continue()
        {
			theResult.AssertWasContinuedToNextBehavior();
        }

    }

    [TestFixture]
    public class when_authenticating_in_a_partial_always_go_to_next : InteractionContext<AuthenticationFilter>
    {
        private FubuContinuation theResult;

        protected override void beforeEach()
        {
            MockFor<ICurrentChain>().Stub(x => x.IsInPartial()).Return(true);

            MockFor<IAuthenticationService>().Stub(x => x.TryToApply())
                .Return(false);

            theResult = ClassUnderTest.Authenticate();
        }

        [Test]
        public void should_continue()
        {
            theResult.AssertWasContinuedToNextBehavior();
        }
    }
}