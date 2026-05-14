// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

using Newtonsoft.Json;

public class AdvertSearchSummary
    {
        public string __typename { get; set; }
        public string url { get; set; }
        public int totalCount { get; set; }
    }

    public class AutomatedAdDescriptionConfig
    {
        public List<int> allowedCategories { get; set; }
        public bool allowPrivate { get; set; }
        public bool allowProfessional { get; set; }
    }

    public class BoostLabelPppCategories
    {
        public List<object> uaCategories { get; set; }
        public List<object> plCategories { get; set; }
        public List<object> ptCategories { get; set; }
        public List<object> roPrdCategories { get; set; }
        public List<object> roStgCategories { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string label { get; set; }
        public string uriPath { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Category2
    {
        public string id { get; set; }
        public string label { get; set; }
        public string uriPath { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Characteristic
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class CheckoutConsumerQueryTimeout
    {
        public int value { get; set; }
    }

    public class Component
    {
        public string __typename { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public object parentID { get; set; }
        public object description { get; set; }
        public object icon { get; set; }
        public object tag { get; set; }
        public object placeholder { get; set; }
        public DisplayConfig displayConfig { get; set; }
    }

    public class Condition
    {
        public string __typename { get; set; }
        public string filterId { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class ConfigPhotoUpload
    {
        public bool decoupled_resize { get; set; }
        public bool resize { get; set; }
        public double resize_threshold { get; set; }
        public string strategy { get; set; }
        public bool workers { get; set; }
        public int concurrency { get; set; }
    }

    public class CookieComplianceReservoir
    {
        public List<string> cookietest { get; set; }
        public List<string> _cookie_test { get; set; }
        public List<string> cookieBarSeenV2 { get; set; }
        public List<string> cookieBarSeen { get; set; }
        public List<string> eupubconsent { get; set; }

        [JsonProperty("eupubconsent-v2")]
        public List<string> eupubconsentv2 { get; set; }
        public List<string> OptanonAlertBoxClosed { get; set; }
        public List<string> OptanonConsent { get; set; }
        public List<string> OTAdditionalConsentString { get; set; }
        public List<string> vinCounter { get; set; }
        public List<string> last_locations { get; set; }
        public List<string> http_host { get; set; }
        public List<string> ldkey { get; set; }
        public List<string> ldf { get; set; }
        public List<string> id_token { get; set; }
        public List<string> invite { get; set; }
        public List<string> client_id { get; set; }
        public List<string> refresh_token { get; set; }
        public List<string> mobile_default { get; set; }
        public List<string> PHPSESSID { get; set; }
        public List<string> laquesis { get; set; }
        public List<string> laquesisqa { get; set; }
        public List<string> laquesis_result { get; set; }
        public List<string> laquesisff { get; set; }
        public List<string> laquesissu { get; set; }
        public List<string> lqonap { get; set; }
        public List<string> lqstatus { get; set; }
        public List<string> test { get; set; }
        public List<string> ock { get; set; }
        public List<string> onap { get; set; }
        public List<string> onap_dev { get; set; }
        public List<string> AuthState { get; set; }
        public List<string> SID { get; set; }
        public List<string> IDPState { get; set; }
        public List<string> LogoutState { get; set; }
        public List<string> ldctx { get; set; }
        public List<string> ldff { get; set; }
        public List<string> datadome { get; set; }

        [JsonProperty("aws-waf-token")]
        public List<string> awswaftoken { get; set; }
        public List<string> thememode { get; set; }
        public List<string> _ga { get; set; }
        public List<string> dfp_user_id { get; set; }
        public List<string> financing { get; set; }
        public List<string> askForFinancing { get; set; }
        public List<string> ignoredDidYouMeanSuggestions { get; set; }
        public List<string> posting_notice { get; set; }
        public List<string> recommended_number_photos { get; set; }
        public List<string> wasThisUsefulQueries { get; set; }
        public List<string> sceuidjs { get; set; }

        [JsonProperty("Force-Experiment-Bypass")]
        public List<string> ForceExperimentBypass { get; set; }
        public List<string> user_id { get; set; }
        public List<string> is_tablet { get; set; }
        public List<string> SESSID { get; set; }
        public List<string> token { get; set; }
        public List<string> offersSubscriptionWidgetSubscribed { get; set; }
        public List<string> subscribedoffersnewsletter { get; set; }
        public List<string> new_packages_messages { get; set; }
        public List<string> upgrades_package_messages { get; set; }
        public List<string> i2_listing { get; set; }
        public List<string> monetizationdefault { get; set; }
        public List<string> myaccount_prolong_click { get; set; }
        public List<string> myaccount_promote_click { get; set; }
        public List<string> ppt { get; set; }
        public List<string> i2_seen_swipe_hint { get; set; }
        public List<string> i2_location { get; set; }
        public List<string> i2_view { get; set; }
        public List<string> price_evaluation { get; set; }
        public List<string> hide_highlights { get; set; }
        public List<string> hide_ad_strength_tutorial { get; set; }
        public List<string> dismissed_carsmile_banner { get; set; }
        public List<string> dismissed_car_valuation { get; set; }
        public List<string> dismissed_covid19_banner { get; set; }
        public List<string> dismissed_klik_banner { get; set; }
        public List<string> dismissed_new_cars_banner { get; set; }
        public List<string> AWSALB { get; set; }
        public List<string> AccessToken { get; set; }
        public List<string> hide_could_be_your_ad { get; set; }
        public List<string> dismissed_relevance_sorting_onboarding { get; set; }
        public List<string> dismissed_vas_recommendation_ads_ids { get; set; }
        public List<string> ads_display_type { get; set; }

        [JsonProperty("ab.storage.userId.0d8dd5cd-3d58-414d-960d-884f3829888b")]
        public List<string> abstorageuserId0d8dd5cd3d58414d960d884f3829888b { get; set; }

        [JsonProperty("ab.storage.userId.e445935b-777a-429f-9f37-ac9297914d6e")]
        public List<string> abstorageuserIde445935b777a429f9f37ac9297914d6e { get; set; }

        [JsonProperty("ab.storage.userId.b36954ae-7ee1-401f-bbd3-b1aa532d1911")]
        public List<string> abstorageuserIdb36954ae7ee1401fbbd3b1aa532d1911 { get; set; }
        public List<string> nsr { get; set; }
    }

    public class DescriptionClassification
    {
        public int delay { get; set; }
        public bool enable { get; set; }
        public List<int> questions { get; set; }
    }

    public class DisplayConfig
    {
        public string __typename { get; set; }
        public string renderAs { get; set; }
        public object hasMultiple { get; set; }
        public string inputMode { get; set; }
        public object suffix { get; set; }
    }

    public class EnablePhotoSizeLimit
    {
        public bool enabled { get; set; }
        public int size { get; set; }
        public bool skipCopy { get; set; }
    }

    public class EnablePirEntrypoint
    {
        public bool enable { get; set; }
    }

    public class EnablePriceDropFirstDeliveryV2
    {
        public bool enabled { get; set; }
    }

    public class ExperimentNationalPhoneNumberWhitelist
    {
        public bool enable { get; set; }
    }

    public class Experiments
    {
        public HomepageRedesign homepage_redesign { get; set; }
        public HomepageRedesignMweb homepage_redesign_mweb { get; set; }
    }

    public class ExperimentWhatsappContactButtonWhitelist
    {
        public bool enable { get; set; }
        public List<string> includedSellers { get; set; }
        public string siteCode { get; set; }
    }

    public class FeatureFlags
    {
        public bool start_from_last_ad { get; set; }
        public bool disable_posting_photo_duplication_check { get; set; }

        [JsonProperty("new_financing_listing_card_entrypoint-native_redirect")]
        public bool new_financing_listing_card_entrypointnative_redirect { get; set; }
        public bool new_ad_search_promoted_page { get; set; }
        public bool enable_new_selling_options { get; set; }
        public ExperimentWhatsappContactButtonWhitelist experiment_whatsapp_contact_button_whitelist { get; set; }
        public bool show_invoices_opt_in_banner { get; set; }

        [JsonProperty("enable-photo-crop")]
        public bool enablephotocrop { get; set; }
        public bool show_info_message { get; set; }

        [JsonProperty("decrypt-sensitive-data-business-site")]
        public bool decryptsensitivedatabusinesssite { get; set; }
        public bool ad_to_video_management { get; set; }

        [JsonProperty("genai-chat-enabled-pages")]
        public GenaiChatEnabledPages genaichatenabledpages { get; set; }
        public bool use_heart_icon_for_favourite_ad { get; set; }
        public bool flag_download_pdf_invoice_gql { get; set; }
        public bool enable_appraisal_tool { get; set; }
        public bool feature_flag_baxter_react_slots_integration { get; set; }
        public bool enable_ads_management_active_access { get; set; }
        public bool inventory_management { get; set; }
        public bool sourcing_tool_bca_tab { get; set; }
        public bool new_monetization_model_myads_counter { get; set; }

        [JsonProperty("leads-management")]
        public bool leadsmanagement { get; set; }
        public bool catalog_enable_stepper { get; set; }
        public bool enable_parts_settings_advert_page { get; set; }

        [JsonProperty("new_financing_header_entrypoint-native_redirect")]
        public bool new_financing_header_entrypointnative_redirect { get; set; }
        public bool otomoto_lease_virtual_category_cta_experiment { get; set; }

        [JsonProperty("motors.account.dealerships-decouple")]
        public bool motorsaccountdealershipsdecouple { get; set; }
        public bool new_ad_search_promoted_ads { get; set; }
        public bool physical_inspection_report_ad_page_report { get; set; }
        public bool new_index_for_cvt_listing_slot { get; set; }

        [JsonProperty("boost-label-ppp-categories")]
        public BoostLabelPppCategories boostlabelpppcategories { get; set; }
        public bool ads_management_vas_button_popover_test { get; set; }
        public int ttl_listing_cache { get; set; }
        public bool enable_ads_management_recommendation_banner { get; set; }
        public bool enable_ads_management_access { get; set; }
        public string listing_new_sorting_experiment { get; set; }
        public bool hide_stock_insights_link_to_ads { get; set; }

        [JsonProperty("financing-simulator_ad_page")]
        public bool financingsimulator_ad_page { get; set; }

        [JsonProperty("financing-ad-sticky-bar_ad_page")]
        public bool financingadstickybar_ad_page { get; set; }

        [JsonProperty("genai-chat-catchup-enabled")]
        public GenaiChatCatchupEnabled genaichatcatchupenabled { get; set; }

        [JsonProperty("enable-vdp-pt")]
        public bool enablevdppt { get; set; }
        public bool enable_ads_active_private_users { get; set; }
        public bool inventory_management_delete_deactivate_actions { get; set; }
        public bool financing_dealer_financing_configurator_page_active { get; set; }
        public bool posting_screen_components_cars { get; set; }

        [JsonProperty("use-adjust-sdk")]
        public bool useadjustsdk { get; set; }
        public bool enable_invoices_page_down_privs { get; set; }
        public bool financing_cofidis_expand_simulation { get; set; }
        public bool enable_new_ad_query_preview { get; set; }
        public bool early_access_experiment { get; set; }

        [JsonProperty("enable-pir-landing-page")]
        public bool enablepirlandingpage { get; set; }
        public bool vehicle_history_report { get; set; }
        public bool enable_professionals_saved_searches { get; set; }
        public bool enable_progressive_disclosure { get; set; }
        public bool enable_ads_management_inactive_filters { get; set; }
        public bool checkout_page_new_monetization_flow_pl_pt { get; set; }
        public bool financing_leasing_listing_carousel { get; set; }
        public bool contact_reasons_filter { get; set; }
        public bool enable_financing_redirects_to_otomotopay { get; set; }
        public bool dashboard_v2_enabled { get; set; }
        public bool video_2_ad_error_feedback { get; set; }

        [JsonProperty("carsmile-sub-price-view")]
        public bool carsmilesubpriceview { get; set; }
        public bool remove_hotjar_inert_attribute { get; set; }
        public bool checkout_page_discount_tags { get; set; }
        public bool enable_beta_ninja_script { get; set; }

        [JsonProperty("leads-advert-filter")]
        public bool leadsadvertfilter { get; set; }
        public bool enable_dealer_dashboard { get; set; }
        public bool show_cepik_extra_information { get; set; }
        public bool listing_contact_ctas_on_ad_card_experiment_enabled { get; set; }

        [JsonProperty("package-comparison-page")]
        public bool packagecomparisonpage { get; set; }

        [JsonProperty("flag-bracket-counter-v2")]
        public bool flagbracketcounterv2 { get; set; }
        public bool csp_report_process { get; set; }
        public bool enable_personalised_payment_cta { get; set; }
        public bool flag_use_ad_v2_layout { get; set; }
        public bool chat_on_ad_page { get; set; }
        public bool enable_new_filter_dropdown { get; set; }
        public bool enable_homepage_cache_new_users { get; set; }
        public bool show_new_cars_autovitro_advert_details { get; set; }

        [JsonProperty("financing-ad_page-entry-points")]
        public bool financingad_pageentrypoints { get; set; }
        public NewCarsLeadsManagement new_cars_leads_management { get; set; }
        public bool benefits_center_page { get; set; }
        public bool show_demand_notification { get; set; }

        [JsonProperty("dealer-financing-batch-enablement-advert-details")]
        public bool dealerfinancingbatchenablementadvertdetails { get; set; }
        public bool enable_invoices_page_down_pros { get; set; }
        public bool enable_total_calls_on_adverts { get; set; }
        public bool enable_genai_disruption_ads_management_chat { get; set; }

        [JsonProperty("genai-chat-extend-enabled")]
        public GenaiChatExtendEnabled genaichatextendenabled { get; set; }

        [JsonProperty("financing-form-canned-response-field-android-webview")]
        public bool financingformcannedresponsefieldandroidwebview { get; set; }
        public bool photo_first_posting_flow_vin_request { get; set; }
        public bool new_checkout_page_composition { get; set; }
        public List<object> financing_multibank_enabled { get; set; }
        public bool ad_to_video_regeneration { get; set; }
        public bool amplify_logout_with_tracking_clear { get; set; }
        public bool postpaid_opt_in_phase_2 { get; set; }
        public bool enable_fake_door_test { get; set; }

        [JsonProperty("genai-chat-drag-and-drop-enabled")]
        public GenaiChatDragAndDropEnabled genaichatdraganddropenabled { get; set; }
        public bool enable_ad_strength { get; set; }

        [JsonProperty("direct-canonical-url")]
        public bool directcanonicalurl { get; set; }

        [JsonProperty("enable-vdp-ro")]
        public bool enablevdpro { get; set; }
        public bool dealer_ratings_my_account_reviews_ask_for_reviews_tab { get; set; }
        public bool checkout_page_revamp_vas_adoption { get; set; }
        public bool posting_url_decoder { get; set; }
        public bool enable_redirect_url_on_authentication { get; set; }

        [JsonProperty("financing-widgets-autovit_redirect")]
        public bool financingwidgetsautovit_redirect { get; set; }
        public bool show_new_cepik_banner { get; set; }
        public bool car_catalog_plate_decoder { get; set; }
        public bool enable_insights_navigation { get; set; }
        public bool enable_pir_in_confirmation_page { get; set; }

        [JsonProperty("listing-relevance-aa-experiment")]
        public string listingrelevanceaaexperiment { get; set; }
        public bool homepage_categories_link_fix { get; set; }
        public bool dealer_ratings_ad_page_skip_empty_state { get; set; }
        public bool use_node_fetch { get; set; }

        [JsonProperty("motors.account.seller-profile-page-for-everyone")]
        public bool motorsaccountsellerprofilepageforeveryone { get; set; }
        public bool cepik_ad_page_partially_verified { get; set; }
        public bool comparison_page_price_tier_update { get; set; }
        public bool enable_listing_lazy_rendering { get; set; }

        [JsonProperty("enable-postpaid-on-stv")]
        public bool enablepostpaidonstv { get; set; }
        public bool feature_flag_graph_ql_tada_enabled_fe_queries { get; set; }
        public bool enable_pro_listing_link_event_fix { get; set; }
        public bool enable_market_data_price_valuation { get; set; }
        public bool uploading_photos_notification { get; set; }
        public bool checkout_performance_breadcrumbs { get; set; }

        [JsonProperty("next-auth")]
        public bool nextauth { get; set; }
        public bool load_equipment_manually { get; set; }
        public bool enable_cepik_banners { get; set; }
        public bool posting_delay_login { get; set; }
        public bool show_new_cars_carspt_homepage { get; set; }
        public bool enable_adcard_vas_recommendation { get; set; }
        public bool dealer_financing_for_carspt_enabled { get; set; }
        public bool enable_score_one { get; set; }
        public bool checkout_page_invoice_graphql { get; set; }
        public bool enable_advert_lazy_layout_component { get; set; }

        [JsonProperty("enable-new-pir-section")]
        public bool enablenewpirsection { get; set; }

        [JsonProperty("featured-dealer-in-vas-multipay")]
        public bool featureddealerinvasmultipay { get; set; }

        [JsonProperty("genai-chat-market-insights-enabled")]
        public bool genaichatmarketinsightsenabled { get; set; }

        [JsonProperty("financing-flow-native-flow-carspt")]
        public bool financingflownativeflowcarspt { get; set; }
        public bool enable_sorting_on_sourcing_tool { get; set; }
        public bool enable_listing_new_cvt_ui { get; set; }
        public bool ai_messaging_launch_banner { get; set; }
        public bool flag_saved_search_queries { get; set; }
        public bool physical_inspection_report_listing_page_filter { get; set; }
        public bool checkout_plutus_ro_provider { get; set; }
        public bool checkout_express_renewals { get; set; }
        public bool use_calltracking_poc { get; set; }
        public bool enable_error_wizard_and_required_fields { get; set; }

        [JsonProperty("bracket-counter-widget-experiment")]
        public bool bracketcounterwidgetexperiment { get; set; }
        public bool featured_dealer_listing_buy_banner { get; set; }
        public bool pro_signUp_activation_on_carspt { get; set; }
        public bool video2ad_posting_flow { get; set; }
        public bool REDIRECT_TO_NEW_ONE_CLICK_VAS_CONFIRMATION_PAGE { get; set; }
        public bool enable_whatsapp_engagement_metric { get; set; }
        public bool physical_inspection_report_favorites_entrypoint { get; set; }

        [JsonProperty("4th_package_parts_enabled")]
        public bool _4th_package_parts_enabled { get; set; }
        public bool vehicle_history_report_imported_vehicles_only { get; set; }
        public bool physical_inspection_report_gallery { get; set; }
        public bool enable_use_file_reader { get; set; }
        public bool enable_move_photos_first_position { get; set; }
        public bool use_calltracking_poc_featured { get; set; }
        public bool checkout_blur_license_plate { get; set; }
        public bool posting_screen_components_parts { get; set; }
        public bool flag_hide_favorites_for_unauthenticated_users { get; set; }
        public bool two_factor_external_link { get; set; }
        public bool lead_source_tips { get; set; }

        [JsonProperty("show-counters-navigation")]
        public bool showcountersnavigation { get; set; }

        [JsonProperty("chat-real-time-read-status")]
        public bool chatrealtimereadstatus { get; set; }
        public bool filter_order_use_listing_v2_screen { get; set; }

        [JsonProperty("klik-horizontal-banner-with-promotion")]
        public bool klikhorizontalbannerwithpromotion { get; set; }

        [JsonProperty("genai-compare-offer-enabled")]
        public GenaiCompareOfferEnabled genaicompareofferenabled { get; set; }
        public bool flag_enable_due_reminder_banners { get; set; }
        public bool cepik_ad_page_timeline { get; set; }
        public bool automated_image_recognizer { get; set; }
        public bool cfp_laquesis_new_integration { get; set; }
        public bool flag_save_similar_search_widget_experiment { get; set; }
        public bool enable_csv_download { get; set; }

        [JsonProperty("enable-ad-detail-pir-preview")]
        public bool enableaddetailpirpreview { get; set; }
        public bool additional_services_migration { get; set; }
        public bool flag_enable_renewals_tip { get; set; }
        public bool enable_ads_management_manage_ads { get; set; }
        public bool enable_auth_middleware_page_filtering { get; set; }
        public bool enable_car_vertical_pl { get; set; }
        public bool enable_graphql_default_client { get; set; }
        public bool enable_vehicle_data_match { get; set; }

        [JsonProperty("4th_package_enabled")]
        public bool _4th_package_enabled { get; set; }
        public bool enable_previous_ad_listing { get; set; }

        [JsonProperty("genai-chat-contextual-help-enabled")]
        public GenaiChatContextualHelpEnabled genaichatcontextualhelpenabled { get; set; }
        public bool chat_filter_product_tour { get; set; }
        public bool enable_vas_promotions_in_ad_stats { get; set; }
        public bool ratings_onboarding { get; set; }
        public bool verified_inventory_posting_form { get; set; }
        public bool enable_assign_sales_person_modal_my_ads { get; set; }
        public bool enable_ads_management_table_view { get; set; }
        public bool checkout_upsell_notifications { get; set; }
        public bool automated_ad_filler_for_license_plate_experiment { get; set; }
        public bool feature_flag_move_queries_fe_enabled { get; set; }

        [JsonProperty("dealer-financing-batch-enablement-listing")]
        public bool dealerfinancingbatchenablementlisting { get; set; }
        public bool enable_highlight_checklist { get; set; }

        [JsonProperty("temporary-flag-for-unicredit-test_autovitro_redirect")]
        public bool temporaryflagforunicredittest_autovitro_redirect { get; set; }
        public bool enable_simple_uploader_in_apps { get; set; }

        [JsonProperty("enable-posting-ssr")]
        public bool enablepostingssr { get; set; }
        public NewCarsListingEntrypoint new_cars_listing_entrypoint { get; set; }
        public bool recommended_number_photos { get; set; }
        public bool enable_new_dashboard_header { get; set; }
        public bool enable_multi_make_model_filter { get; set; }
        public bool enable_clear_fields_decoder_fails { get; set; }
        public bool enable_topup_decoupling { get; set; }
        public bool access_control_salesperson_profile { get; set; }
        public bool show_click2buy_ads_on_listing_page { get; set; }
        public CheckoutConsumerQueryTimeout checkout_consumer_query_timeout { get; set; }
        public bool dealer_ratings_onboarding { get; set; }
        public bool wallet_as_default_payment_method { get; set; }
        public bool enable_add_additional_headers_apollo { get; set; }
        public bool enable_financing_loan_simulation_gql_integration { get; set; }
        public bool ad_to_video_verify_benefit_and_visibility { get; set; }
        public bool enable_redirect_ad_submit_authentication_fails { get; set; }
        public bool enable_market_data_single_initial_query { get; set; }
        public bool photo_first_posting_flow_private { get; set; }
        public bool disable_car_parts_pt_category { get; set; }
        public bool skip_category_selector { get; set; }
        public string vehicle_insights_ad_page_submission_email { get; set; }
        public bool dealer_ratings_dark_launch { get; set; }
        public bool pricing_insights_car_valuation { get; set; }
        public bool enable_decrypt_seller_uuid { get; set; }
        public bool enable_pro_listing_bca_content { get; set; }
        public bool new_financing_ad_car_details_entrypoint { get; set; }
        public EnablePriceDropFirstDeliveryV2 enable_price_drop_first_delivery_v2 { get; set; }
        public bool enable_logging_exchange { get; set; }
        public bool next_image_in_listing_page { get; set; }

        [JsonProperty("enable-pir-pii-gallery")]
        public bool enablepirpiigallery { get; set; }
        public bool enable_inventory_quick_filter_make { get; set; }
        public bool enable_market_data_beta_tag { get; set; }

        [JsonProperty("enable-extended-versions")]
        public bool enableextendedversions { get; set; }
        public bool messaging_fraud_typing_tooltip { get; set; }

        [JsonProperty("financing-header_cta")]
        public bool financingheader_cta { get; set; }
        public bool ads_management_vas_button_popover { get; set; }

        [JsonProperty("new_financing_observed_ads-native_redirect")]
        public bool new_financing_observed_adsnative_redirect { get; set; }
        public bool financing_simulation_banner { get; set; }
        public bool enable_new_ad_stats_sidebar { get; set; }

        [JsonProperty("genai-chat-unanswered-messages-enabled")]
        public GenaiChatUnansweredMessagesEnabled genaichatunansweredmessagesenabled { get; set; }
        public bool enable_clean_system_description { get; set; }
        public bool show_click2buy_banner_listing { get; set; }
        public SourcingFiltersLayoutFix sourcing_filters_layout_fix { get; set; }

        [JsonProperty("financing-listing-otomoto-lease-banner")]
        public bool financinglistingotomotoleasebanner { get; set; }
        public bool dealer_ratings_ad_page { get; set; }
        public bool new_financing_observed_ads_entrypoint { get; set; }
        public bool contact_reason_removal { get; set; }
        public bool my_account_parts_settings_tab { get; set; }

        [JsonProperty("reuse-cookie-to-reduce-size")]
        public bool reusecookietoreducesize { get; set; }

        [JsonProperty("active-sessions-settings-menu")]
        public bool activesessionssettingsmenu { get; set; }
        public bool laquesis { get; set; }
        public bool my_account_reviews_onboarding { get; set; }
        public PriceDropExperimentOptOut price_drop_experiment_opt_out { get; set; }
        public bool checkout_plutus_provider { get; set; }
        public bool dealer_ratings_feedback_form { get; set; }

        [JsonProperty("financing-widgets-autovitro_redirect")]
        public string financingwidgetsautovitro_redirect { get; set; }
        public bool listing_page_ad_card_next_link { get; set; }
        public bool enable_bracket_counter_banner { get; set; }
        public bool show_new_cars_otomotopl_homepage { get; set; }

        [JsonProperty("conversation-lead-qualification-survey")]
        public bool conversationleadqualificationsurvey { get; set; }
        public bool enable_market_view_menu { get; set; }
        public bool new_financing_footer_entrypoint { get; set; }
        public bool cepik_listing_page { get; set; }

        [JsonProperty("listing-tab-change-filter-validation")]
        public bool listingtabchangefiltervalidation { get; set; }

        [JsonProperty("enable-could-be-your-ad-flow")]
        public bool enablecouldbeyouradflow { get; set; }
        public bool price_tier { get; set; }
        public NewCarsAdvertRedirectEntrypoint new_cars_advert_redirect_entrypoint { get; set; }

        [JsonProperty("carsmile-horizontal-banner")]
        public bool carsmilehorizontalbanner { get; set; }
        public bool ENABLE_CHECKOUT_VAS_FILTER_INVALID_PRICES { get; set; }
        public bool automated_ad_filler_for_license_plate { get; set; }
        public bool ads_management_use_get_user_session { get; set; }
        public bool show_preview_seller_actions { get; set; }
        public bool my_account_reviews_tab_new_tag { get; set; }
        public bool enable_invoices_page_down { get; set; }
        public bool enable_professionals_listing_page { get; set; }
        public bool enable_wallet_topup_user { get; set; }
        public bool enable_new_cars_catalog_page { get; set; }
        public bool enable_professional_saved_searches_tab { get; set; }

        [JsonProperty("financing-picker-button-ro")]
        public bool financingpickerbuttonro { get; set; }

        [JsonProperty("financing-use-app-platform-identification-to-redirect")]
        public bool financinguseappplatformidentificationtoredirect { get; set; }

        [JsonProperty("all-leads-aggregation")]
        public bool allleadsaggregation { get; set; }
        public bool financing_forms_enable_applying_as_field { get; set; }
        public bool enable_discount_banner { get; set; }
        public ExperimentNationalPhoneNumberWhitelist experiment_national_phone_number_whitelist { get; set; }
        public string encrypt_sensitive_data_version { get; set; }
        public bool show_messages_tab { get; set; }
        public bool enable_white_background_footer_professional { get; set; }
        public bool checkout_enable_long_duration_package { get; set; }
        public bool enable_posting_in_app { get; set; }
        public bool my_ads_draft_rollout_feature { get; set; }
        public bool my_account_reviews_tab { get; set; }

        [JsonProperty("klik-horizontal-banner")]
        public bool klikhorizontalbanner { get; set; }
        public bool benefits_center_upgrade_items_v2 { get; set; }
        public bool flag_disable_atlas_homepage_ads { get; set; }
        public bool enable_redirect_if_session_fails_submission { get; set; }
        public bool dealer_financing_on_3rd_package { get; set; }

        [JsonProperty("enable-banner-effective-sap-cutover-pros")]
        public bool enablebannereffectivesapcutoverpros { get; set; }
        public bool use_search_max_age { get; set; }
        public bool overview_page_rollout_feature { get; set; }
        public bool cepik_ad_page_extra_information { get; set; }
        public bool flag_vas_bundles_oto_update { get; set; }
        public bool dealer_financing_forms_cofidis { get; set; }
        public bool enable_banner_link_to_dashboard { get; set; }
        public bool enable_search_summary_sas { get; set; }
        public bool listing_new_filters_for_parts { get; set; }
        public bool activate_service_benefits { get; set; }
        public bool chat_release_2_tabs { get; set; }
        public bool straight_post_form_link { get; set; }
        public bool checkout_read_next_auth_session_user_type { get; set; }

        [JsonProperty("advert-page-404-mismatch-categories")]
        public bool advertpage404mismatchcategories { get; set; }
        public bool show_new_tag_on_my_profile_tab { get; set; }

        [JsonProperty("chat-real-time-chat-typing")]
        public bool chatrealtimechattyping { get; set; }

        [JsonProperty("enable-banner-effective-sap-cutover-privs")]
        public bool enablebannereffectivesapcutoverprivs { get; set; }
        public bool enable_show_header_cvp { get; set; }
        public bool enable_ads_management_moderated_access { get; set; }
        public bool enable_same_upload_strategy { get; set; }
        public bool enable_bca_filters_on_sourcing_tool { get; set; }

        [JsonProperty("financing-listing_ad_card")]
        public bool financinglisting_ad_card { get; set; }
        public bool klik_new_contact_form { get; set; }
        public bool debug_business_site_routing { get; set; }
        public bool physical_inspection_report_financing_widget { get; set; }
        public bool description_rich_text_input { get; set; }
        public bool enable_market_data_mobile { get; set; }
        public bool feature_flag_related_ads_sidebard_enabled { get; set; }

        [JsonProperty("delete-account")]
        public bool deleteaccount { get; set; }
        public bool checkout_repeated_tax_id_validation { get; set; }
        public bool chat_release_4 { get; set; }
        public bool posting_detect_photo_angles { get; set; }

        [JsonProperty("financing-widgets-carspt_redirect")]
        public string financingwidgetscarspt_redirect { get; set; }
        public bool enable_temporary_safe_redirect { get; set; }
        public bool physical_inspection_report { get; set; }
        public bool enable_ksef_banner_invoice { get; set; }

        [JsonProperty("genai-chat-show-market-insights-enabled")]
        public GenaiChatShowMarketInsightsEnabled genaichatshowmarketinsightsenabled { get; set; }

        [JsonProperty("genai-chat-unpaid-enabled")]
        public GenaiChatUnpaidEnabled genaichatunpaidenabled { get; set; }

        [JsonProperty("ewallet-unavailable-disclaimer-message")]
        public bool ewalletunavailabledisclaimermessage { get; set; }
        public bool dealer_financing_forms_santander { get; set; }
        public bool new_financing_ad_under_price_entrypoint { get; set; }
        public bool ciam_backward_compatibility { get; set; }

        [JsonProperty("show-discount-percentage-in-checkout")]
        public bool showdiscountpercentageincheckout { get; set; }
        public bool posting_auto_sort_photo { get; set; }
        public bool delight_retain_new_navigation_system { get; set; }
        public bool new_relic_test_feature_flag { get; set; }
        public bool enable_change_translation_entrypoint_lp { get; set; }
        public bool posting_blur_license_plate { get; set; }
        public bool dealer_ratings_ad_page_reviews_side_panel { get; set; }
        public bool ai_generated_messages { get; set; }
        public bool christmas_logo { get; set; }
        public bool listing_page_seller_ratings { get; set; }
        public bool enable_ads_management_archived_access { get; set; }

        [JsonProperty("klik-detail-page-client-navigation-from-listing")]
        public bool klikdetailpageclientnavigationfromlisting { get; set; }
        public bool enable_fd_card_buy_button { get; set; }

        [JsonProperty("dealer-financing-settings-request-enabled")]
        public bool dealerfinancingsettingsrequestenabled { get; set; }
        public bool dealer_ratings_advert_deactivation_request_rating { get; set; }
        public bool dealer_ratings_readonly_ratings_distribution { get; set; }
        public bool dealer_ratings_reply { get; set; }
        public bool catalog_enable_ad_preview { get; set; }
        public bool automated_ad_description { get; set; }
        public int flag_ttl_advertpage_cache { get; set; }

        [JsonProperty("experiment-required-form-control")]
        public bool experimentrequiredformcontrol { get; set; }

        [JsonProperty("genai-chat-estimate-enabled")]
        public GenaiChatEstimateEnabled genaichatestimateenabled { get; set; }
        public bool enable_multi_make_model { get; set; }
        public bool financing_ad_page_entrypoint_manager { get; set; }
        public bool enable_ads_management_promotion_status { get; set; }

        [JsonProperty("description-classification")]
        public DescriptionClassification descriptionclassification { get; set; }
        public bool enable_new_summary_schema { get; set; }
        public bool browser_metrics { get; set; }

        [JsonProperty("enable-catalog-debounce")]
        public bool enablecatalogdebounce { get; set; }
        public bool show_electric_vehicles_page { get; set; }
        public bool flag_enable_maas_on_my_active_ads { get; set; }
        public bool enable_ads_management_active_vas { get; set; }
        public bool enable_parts_compatibility { get; set; }
        public bool enable_remove_products_without_price { get; set; }
        public bool calls_table_revamp { get; set; }
        public bool enable_stock_overview_banner { get; set; }
        public bool enable_ads_management_notifications_banner { get; set; }
        public bool new_financing_header_entrypoint { get; set; }
        public bool chat_release_1 { get; set; }
        public bool new_benefits_menu { get; set; }
        public bool early_access_banner_experiment { get; set; }
        public bool dealer_ratings_my_account_onboarding_automatic_trigger { get; set; }

        [JsonProperty("financing-form-reason-for-contact-field-ios-webview")]
        public bool financingformreasonforcontactfieldioswebview { get; set; }
        public bool enable_financing_black_november_bcr { get; set; }
        public bool photo_captions_usefulness { get; set; }
        public bool feature_flag_baxter_after_interactive_strategy { get; set; }
        public PhysicalInspectionReportRegionsRestriction physical_inspection_report_regions_restriction { get; set; }
        public bool consumer_multipay_vas_prediction { get; set; }
        public bool enable_new_advert_page_gallery { get; set; }
        public bool show_cepik_badge { get; set; }
        public bool react_compiler { get; set; }
        public bool vehicle_history_report_campaign { get; set; }
        public bool enable_straight_upload_in_apps { get; set; }
        public bool enable_stock_overview_onboarding_app { get; set; }
        public bool feature_flag_audit { get; set; }
        public bool hosted_ciam_integration { get; set; }
        public bool enable_pir_for_professionals { get; set; }
        public bool enable_search_summary_sas_listing { get; set; }
        public bool car_catalog_vin_decoder { get; set; }
        public bool enable_seller_financing_page { get; set; }

        [JsonProperty("enable-posting-chrome-translation")]
        public bool enablepostingchrometranslation { get; set; }
        public bool financing_forms_dealer_financing_otomoto_native { get; set; }
        public bool new_financing_ad_gallery_entrypoint { get; set; }
        public bool clean_session_cookie_no_id_refresh_token { get; set; }
        public bool listing_page_ad_card_redesign { get; set; }
        public bool premium_dealer_ad_details_page { get; set; }
        public bool hidden_contact_number_footer { get; set; }

        [JsonProperty("enable-market-data-link-new-badge")]
        public bool enablemarketdatalinknewbadge { get; set; }
        public bool message_form_redirect_to_new_conversation { get; set; }
        public bool checkout_page_invoice_region_field_c2c { get; set; }

        [JsonProperty("package-comparison-page-down")]
        public bool packagecomparisonpagedown { get; set; }
        public bool enable_appraisal_tool_beta_tag { get; set; }
        public bool enable_ratings_for_buyers { get; set; }

        [JsonProperty("financing-routing-engine-migration")]
        public bool financingroutingenginemigration { get; set; }
        public bool enable_in_app_browser { get; set; }
        public bool flag_use_ad_v3_layout { get; set; }

        [JsonProperty("flag-invoice-payment-enabled")]
        public bool flaginvoicepaymentenabled { get; set; }
        public bool show_new_tag_on_sourcing_tab { get; set; }
        public bool dealer_ratings_report_review { get; set; }
        public bool ad_to_video_advert_details { get; set; }
        public bool physical_inspection_report_discount { get; set; }
        public bool enable_page_caching_optimus { get; set; }
        public bool enable_vin_behind_login_experiment { get; set; }
        public bool enable_ads_management_bulk_renew { get; set; }
        public bool enable_ads_management_delete_all { get; set; }
        public bool enable_ads_management_see_more { get; set; }

        [JsonProperty("genai-chat-unanswered-calls-enabled")]
        public GenaiChatUnansweredCallsEnabled genaichatunansweredcallsenabled { get; set; }

        [JsonProperty("temporary-flag-for-description-cta_autovitro_redirect")]
        public string temporaryflagfordescriptioncta_autovitro_redirect { get; set; }
        public bool enable_self_subscription_of_early_access { get; set; }
        public bool enable_resize_in_web_view { get; set; }
        public bool delight_retain_leads_from_favs_buyers { get; set; }
        public bool use_custom_input_in_ranges { get; set; }

        [JsonProperty("enable-vdp-pl")]
        public bool enablevdppl { get; set; }
        public bool posting_screen_components { get; set; }
        public bool dealer_ratings_my_account_reviews_tab { get; set; }
        public bool physical_inspection_report_multi_package { get; set; }

        [JsonProperty("financing-widgets-comperia-simulation")]
        public bool financingwidgetscomperiasimulation { get; set; }
        public bool enable_ads_pending_private_users { get; set; }
        public bool show_new_entrypoint_desktop { get; set; }
        public bool bs_inventory_new_initial_data { get; set; }
        public bool feature_use_password_meter { get; set; }

        [JsonProperty("enable-pir-pii-banner")]
        public bool enablepirpiibanner { get; set; }
        public bool posting_enable_late_login { get; set; }
        public bool enable_immediate_migration_package_comparison { get; set; }

        [JsonProperty("new-error-page")]
        public bool newerrorpage { get; set; }
        public bool enable_add_inventory_appraisal_tool { get; set; }

        [JsonProperty("listing-relevance-experiment")]
        public string listingrelevanceexperiment { get; set; }
        public bool enable_genai_interactive_metrics { get; set; }

        [JsonProperty("horizontal-banner-with-black-friday-promotion")]
        public bool horizontalbannerwithblackfridaypromotion { get; set; }
        public bool video2ad_upload_poc { get; set; }

        [JsonProperty("listing-relevance-experiments")]
        public List<string> listingrelevanceexperiments { get; set; }
        public bool enable_posting_uuid_validation { get; set; }
        public bool enable_market_data_new_tag { get; set; }
        public bool show_new_cars_autovitro_homepage { get; set; }
        public bool chat_release_5 { get; set; }

        [JsonProperty("enable-platinum-plus-ro")]
        public bool enableplatinumplusro { get; set; }
        public bool enable_pro_listing_bca_list { get; set; }
        public bool amplify_logout_with_tracking { get; set; }
        public bool remove_financial_options { get; set; }
        public bool chat_fraud_banner { get; set; }

        [JsonProperty("otomoto-lease-redirect-on-lease-advert")]
        public bool otomotoleaseredirectonleaseadvert { get; set; }
        public bool enable_posting_setup_wait_page_leave { get; set; }
        public bool enable_ads_management_server_breakpoints { get; set; }
        public bool enable_version_survey { get; set; }
        public bool new_cvt_model { get; set; }
        public bool financing_listing_page_representative_example { get; set; }
        public bool enable_new_copy_pir_cluster { get; set; }
        public bool enable_hide_lp_and_rd { get; set; }
        public bool enable_shop_menu_translations { get; set; }
        public bool enable_l1_view_market_data_app { get; set; }
        public bool car_parts_next_invoice_calculator_ui { get; set; }
        public bool physical_inspection_report_fake_door { get; set; }
        public bool posting_form_mask_phone_number { get; set; }
        public bool additional_request_logging_for_load_testing { get; set; }
        public bool enable_ad_stats_olx_data_display { get; set; }

        [JsonProperty("enable-ksef-download")]
        public bool enableksefdownload { get; set; }

        [JsonProperty("motors.multiuser.sidebar_invite_multiuser_banner")]
        public bool motorsmultiusersidebar_invite_multiuser_banner { get; set; }

        [JsonProperty("static-assets-next-image")]
        public bool staticassetsnextimage { get; set; }
        public bool enable_permissions_query_on_ads_management { get; set; }
        public bool enable_filters_machine_non_cars_listing { get; set; }
        public bool messaging_fraud_disclaimer { get; set; }
        public bool enable_ads_management_auto_renew_cta { get; set; }
        public bool cepik_ad_page_only_visible_for_sellers { get; set; }
        public bool enable_vehicle_match_request { get; set; }
        public bool chat_release_2 { get; set; }
        public bool csp_add_headers { get; set; }
        public bool enable_reduced_tooltips_mweb { get; set; }

        [JsonProperty("premium-dealer-badge")]
        public bool premiumdealerbadge { get; set; }

        [JsonProperty("financing-ad-car-details_ad_page")]
        public bool financingadcardetails_ad_page { get; set; }
        public bool checkout_vas_best_seller { get; set; }
        public bool checkout_page_experiment_aa_tests_revenue_metrics { get; set; }
        public bool enable_similar_ads_on_appraisal_tool { get; set; }
        public bool always_update_invoice_enabled { get; set; }
        public bool hide_pes_widgets { get; set; }
        public bool checkout_mask_phone_number { get; set; }
        public bool enable_checkout_vas_15_days { get; set; }
        public bool auto_replies_toggle { get; set; }
        public bool cache_experiment { get; set; }
        public bool enable_car_valuation_in_cvt { get; set; }
        public bool amplify_logout_with_tracking_remove { get; set; }
        public bool dealer_ratings_my_account_feedback_banner { get; set; }
        public bool enable_ads_management_polling { get; set; }
        public bool clear_cognito_cookies { get; set; }
        public bool image2ad_make_model_fallback { get; set; }
        public bool enable_adcard_statistics_link { get; set; }
        public bool show_stock_insights_see_active_ads { get; set; }
        public bool enable_new_fields_cvt { get; set; }
        public bool new_financing_listing_card_entrypoint { get; set; }
        public bool fetch_options_merge_graphql { get; set; }

        [JsonProperty("dealer-financing-benefit-card")]
        public bool dealerfinancingbenefitcard { get; set; }
        public bool enable_entrypoint_pir_my_ads { get; set; }
        public bool enable_market_data_filters { get; set; }
        public bool vehicle_history_report_new_tag { get; set; }

        [JsonProperty("disable-stats-entrypoints-for-private")]
        public bool disablestatsentrypointsforprivate { get; set; }
        public bool enable_popover_saved_searches_listing { get; set; }
        public bool dealer_ratings_ad_page_standout_category { get; set; }
        public bool multipay_fx_rate_message { get; set; }
        public bool enable_ad_card_insights_click { get; set; }
        public bool enable_cepik_in_ad_strength { get; set; }
        public bool ads_impression_tracking_on_viewport_enter { get; set; }
        public bool show_klik_ads_section { get; set; }
        public bool listing_page_new_service_sorting_options_default_enabled { get; set; }

        [JsonProperty("benefits-history")]
        public bool benefitshistory { get; set; }
        public bool chat_release_3 { get; set; }
        public bool allow_embedded_in_iframe { get; set; }

        [JsonProperty("config-photo-upload")]
        public ConfigPhotoUpload configphotoupload { get; set; }
        public bool checkout_plutus_pt_provider { get; set; }
        public bool enables_new_financing_flow { get; set; }
        public bool inject_hive_client_headers { get; set; }
        public bool enable_adcard_ad_profitability_recommendation { get; set; }
        public bool enable_server_side_refresh_tokens { get; set; }

        [JsonProperty("financing-filters-affordability-widget")]
        public bool financingfiltersaffordabilitywidget { get; set; }
        public bool klik_ad_details_page { get; set; }

        [JsonProperty("new_financing_under_price_ad_page-native_redirect")]
        public bool new_financing_under_price_ad_pagenative_redirect { get; set; }
        public bool cepik_ad_page { get; set; }
        public bool enable_market_data { get; set; }
        public bool show_simplified_pf { get; set; }
        public bool dealer_ratings_iovation_fingerprint { get; set; }
        public bool gql_persisted_queries { get; set; }
        public bool checkout_product_vas_scheduler { get; set; }
        public bool ads_management_enable_session_cleanup { get; set; }
        public bool enable_ads_management_advanced_filters { get; set; }
        public bool enable_photos_resize { get; set; }
        public NewCarsAdvertEntrypoint new_cars_advert_entrypoint { get; set; }
        public bool listing_premium_top_ad_enabled { get; set; }
        public bool enable_next_image_advert_gallery { get; set; }
        public bool use_new_currency_rates_on_bank_transfer { get; set; }
        public bool enable_new_ad_query { get; set; }
        public bool financing_simulation_banner_contact_form { get; set; }
        public bool pf_personalized_ctas { get; set; }
        public bool chat_release_6 { get; set; }
        public bool enable_mkt_data_total_leads_fake_door { get; set; }
        public bool enable_pir_entrypoints_fake_door { get; set; }
        public bool confirmation_page_buttons_app { get; set; }
        public bool enable_webview_native_tracking { get; set; }
        public bool show_homepage_video_banner { get; set; }
        public bool use_calltracking_poc_business_site { get; set; }
        public bool enable_favorites_page { get; set; }
        public bool enable_pf_experiment_personalized_ctas_v2 { get; set; }

        [JsonProperty("new_financing_ad_car_details_entrypoint-native_redirect")]
        public bool new_financing_ad_car_details_entrypointnative_redirect { get; set; }
        public bool automatic_ad_filler { get; set; }
        public bool photo_first_posting_flow { get; set; }

        [JsonProperty("financing-ai-advisor-enablement")]
        public bool financingaiadvisorenablement { get; set; }
        public bool show_motopedia_link { get; set; }
        public bool checkout_private_packages_v2_translations { get; set; }
        public bool resize_photos_in_workers { get; set; }
        public bool enable_ads_management_filters { get; set; }
        public bool show_cta_switch_to_old_page { get; set; }
        public bool new_ad_search_offer_of_the_day { get; set; }
        public bool financing_ad_page_representative_example { get; set; }
        public bool enable_show_discounted_price_pir_listing_page { get; set; }
        public bool enable_early_access_pl_restriction { get; set; }
        public bool checkout_b2c_plutus_provider { get; set; }
        public bool enable_iaas_at_professionals_listing { get; set; }
        public bool physical_inspection_report_c2c { get; set; }
        public bool enable_new_sort_options_query { get; set; }
        public bool enable_ninja_2 { get; set; }
        public bool car_catalog_new_cars_features { get; set; }
        public bool enable_ads_management_table_view_limit { get; set; }
        public bool two_step_multipay { get; set; }
        public bool enable_page_caching_optimus_fetcher { get; set; }
        public bool enable_l1_view_dashboard_app { get; set; }
        public bool enable_strict_image_upload { get; set; }
        public bool table_new_scroll_button { get; set; }
        public bool active_ads_count_professionals_listing { get; set; }
        public bool catalog_local_only { get; set; }
        public bool enable_ads_management_deactivate_all { get; set; }
        public bool laquesis_circuit_breaker { get; set; }
        public bool service_ads_search_ro { get; set; }
        public bool my_ads_app_heading { get; set; }

        [JsonProperty("benefit-center-new-overview-tab")]
        public bool benefitcenternewoverviewtab { get; set; }
        public bool csp_enforce { get; set; }

        [JsonProperty("enable-description-sync-attribute")]
        public bool enabledescriptionsyncattribute { get; set; }
        public bool enable_is_redirect_unconfirmed_business_user { get; set; }

        [JsonProperty("financing-under_price_cta_ad_page")]
        public bool financingunder_price_cta_ad_page { get; set; }
        public bool listing_page_new_service_sorting_options { get; set; }
        public bool enable_load_from_auto_save { get; set; }

        [JsonProperty("financing-widgets-otomotopl_redirect")]
        public string financingwidgetsotomotopl_redirect { get; set; }
        public bool masked_input_phone_number { get; set; }
        public bool show_login_wall_cvt { get; set; }

        [JsonProperty("genai-chat-streaming-enabled")]
        public GenaiChatStreamingEnabled genaichatstreamingenabled { get; set; }
        public bool randomise_click2buy_ads_positions { get; set; }
        public bool enable_ads_management_ad_position { get; set; }
        public bool use_undici_request_fetch { get; set; }
        public bool gql_normalized_cache { get; set; }

        [JsonProperty("flag-ad-info-metadata")]
        public bool flagadinfometadata { get; set; }

        [JsonProperty("enable-pir-entrypoint")]
        public EnablePirEntrypoint enablepirentrypoint { get; set; }
        public bool checkout_new_mobile_packages { get; set; }
        public bool enable_new_top_ads_query { get; set; }
        public bool dashboard_stock_insights_benefit_check { get; set; }
        public bool delight_retain_leads_from_favs { get; set; }
        public bool enable_temp_app_l2_view { get; set; }
        public bool hide_car_condition { get; set; }
        public bool listing_page_new_service_sorting_options_2 { get; set; }
        public bool listing_click2buy_carsmile_klik_card_split { get; set; }
        public bool enable_one_trust_cookies_consent_banner { get; set; }
        public bool enable_inventory_quick_filters { get; set; }
        public bool financing_is_new_form_fields_active { get; set; }
        public bool automatic_ad_filler_experiment { get; set; }

        [JsonProperty("new-rental-option")]
        public bool newrentaloption { get; set; }
        public bool new_inbox_screen { get; set; }
        public bool include_submodel_on_professionals_listing { get; set; }
        public bool enable_pro_listing_authentication_condition { get; set; }
        public bool enable_redirect_if_session_fails_images { get; set; }

        [JsonProperty("enable-photo-size-limit")]
        public EnablePhotoSizeLimit enablephotosizelimit { get; set; }

        [JsonProperty("enable-banner-pre-sap-cutover-privs")]
        public bool enablebannerpresapcutoverprivs { get; set; }
        public bool enable_checkout_collapsable_vas { get; set; }

        [JsonProperty("automated-ad-description-config")]
        public AutomatedAdDescriptionConfig automatedaddescriptionconfig { get; set; }
        public bool enable_new_copy_inspected_ads { get; set; }
        public bool maze_survey_script { get; set; }
        public bool enable_simple_uploader { get; set; }
        public bool auth_check_id_token { get; set; }
        public bool enable_search_summary_sas_home { get; set; }
        public bool enable_invoice_csv_through_billing_service { get; set; }
        public bool hide_advert_icon_from_ad_details_page { get; set; }
        public bool enable_ads_management_activate_all { get; set; }
        public bool dealer_ratings_iovation_reverse_proxy { get; set; }

        [JsonProperty("enable-one-trust-cookies-autoblocking")]
        public bool enableonetrustcookiesautoblocking { get; set; }
        public bool gql_compressed_queries { get; set; }
        public bool comparison_page_tier_info_surcharged { get; set; }

        [JsonProperty("genai-dnd-enabled")]
        public GenaiDndEnabled genaidndenabled { get; set; }
        public bool license_plate_date_mandatory { get; set; }

        [JsonProperty("enable-banner-pre-sap-cutover-pros")]
        public bool enablebannerpresapcutoverpros { get; set; }
        public bool enable_homepage_cache { get; set; }
        public bool financing_is_new_dealer_financing_management_page_active { get; set; }
        public bool appraisal_tool_entry_point_on_listing_page { get; set; }
        public string new_cars_redirect { get; set; }
        public bool enable_gql_mutations_package_comparison { get; set; }
        public bool show_tabs_experiment { get; set; }
        public bool collapsible_checkboxes_list { get; set; }
        public bool enable_pir_landing_page { get; set; }
        public bool enable_abuse_report { get; set; }
        public bool delight_retain_edit_saved_search_name { get; set; }
        public bool posting_gross_net { get; set; }
        public bool enable_inactive_ads_high_demand_car_banner { get; set; }
        public bool enable_ads_management_pending_access { get; set; }
        public int ttl_homepage_cache { get; set; }
        public bool use_early_access_policy_on_banner { get; set; }

        [JsonProperty("opt-in-users-first-phase")]
        public bool optinusersfirstphase { get; set; }

        [JsonProperty("genai-chat-compare-offer-enabled")]
        public GenaiChatCompareOfferEnabled genaichatcompareofferenabled { get; set; }
        public bool enable_aside_on_left { get; set; }
        public bool physical_inspection_report_listing_page_carousel { get; set; }
        public bool low_level_logging { get; set; }
        public bool enable_new_dashboard_tabs_in_mobile { get; set; }
        public bool homepage_custom_kamper_category { get; set; }
        public bool new_packaging_page { get; set; }

        [JsonProperty("business-site-pusblished-ads-migration")]
        public bool businesssitepusblishedadsmigration { get; set; }
        public bool listing_page_disable_ssr_exchange { get; set; }
    }

    public class GenaiChatCatchupEnabled
    {
        public bool enable { get; set; }
        public bool mobile { get; set; }
    }

    public class GenaiChatCompareOfferEnabled
    {
        public bool enable { get; set; }
        public bool mobile { get; set; }
    }

    public class GenaiChatContextualHelpEnabled
    {
        public bool mobile { get; set; }
        public bool enable { get; set; }
    }

    public class GenaiChatDragAndDropEnabled
    {
        public bool enable { get; set; }
        public bool mobile { get; set; }
    }

    public class GenaiChatEnabledPages
    {
        public bool mobile { get; set; }
        public List<string> testUsers { get; set; }
        public bool enable { get; set; }
    }

    public class GenaiChatEstimateEnabled
    {
        public bool mobile { get; set; }
        public bool enable { get; set; }
    }

    public class GenaiChatExtendEnabled
    {
        public bool enable { get; set; }
    }

    public class GenaiChatShowMarketInsightsEnabled
    {
        public bool enable { get; set; }
        public bool mobile { get; set; }
    }

    public class GenaiChatStreamingEnabled
    {
        public bool enable { get; set; }
        public bool mobile { get; set; }
    }

    public class GenaiChatUnansweredCallsEnabled
    {
        public bool mobile { get; set; }
        public bool enable { get; set; }
    }

    public class GenaiChatUnansweredMessagesEnabled
    {
        public bool enable { get; set; }
        public bool mobile { get; set; }
    }

    public class GenaiChatUnpaidEnabled
    {
        public bool mobile { get; set; }
        public bool enable { get; set; }
    }

    public class GenaiCompareOfferEnabled
    {
    }

    public class GenaiDndEnabled
    {
    }

    public class Home
    {
        public string cs_hours { get; set; }
        public string cs_number { get; set; }

        [JsonProperty("Customer service")]
        public string Customerservice { get; set; }

        [JsonProperty("Do you need help?")]
        public string Doyouneedhelp { get; set; }

        [JsonProperty("Download mobile app")]
        public string Downloadmobileapp { get; set; }

        [JsonProperty("dropdown-placeholder")]
        public string dropdownplaceholder { get; set; }

        [JsonProperty("electric-cars-landing-page")]
        public string electriccarslandingpage { get; set; }

        [JsonProperty("explore-tools-articles")]
        public string exploretoolsarticles { get; set; }

        [JsonProperty("free-evaluation-minutes")]
        public string freeevaluationminutes { get; set; }

        [JsonProperty("genuine-buyers")]
        public string genuinebuyers { get; set; }

        [JsonProperty("help-selling-title")]
        public string helpsellingtitle { get; set; }

        [JsonProperty("home-experiment-business-site-get-to-know-this-seller")]
        public string homeexperimentbusinesssitegettoknowthisseller { get; set; }

        [JsonProperty("home-experiment-business-site-know-more-to-choose-better")]
        public string homeexperimentbusinesssiteknowmoretochoosebetter { get; set; }

        [JsonProperty("home-experiment-business-site-know-this-seller")]
        public string homeexperimentbusinesssiteknowthisseller { get; set; }

        [JsonProperty("home-offer-of-the-day")]
        public string homeofferoftheday { get; set; }
        public string home_advanced_search_button { get; set; }
        public string home_featured_ads_title { get; set; }
        public string home_filters_error { get; set; }
        public string home_filters_error_retry { get; set; }
        public string home_offer_of_the_day { get; set; }
        public string home_search_title { get; set; }
        public string home_show_results_button { get; set; }
        public string home_view_all_link { get; set; }

        [JsonProperty("listing-ad-card-seller-link")]
        public string listingadcardsellerlink { get; set; }

        [JsonProperty("Mobile app download")]
        public string Mobileappdownload { get; set; }

        [JsonProperty("new-cars")]
        public string newcars { get; set; }

        [JsonProperty("new-cars-description")]
        public string newcarsdescription { get; set; }

        [JsonProperty("new-cars-header")]
        public string newcarsheader { get; set; }
        public string page_home_toplinks_title_car_makes { get; set; }
        public string page_home_toplinks_title_car_models_versions { get; set; }
        public string page_home_toplinks_title_districts { get; set; }
        public string page_home_toplinks_title_models_audi { get; set; }
        public string page_home_toplinks_title_models_bmw { get; set; }
        public string page_home_toplinks_title_models_renault { get; set; }
        public string page_home_toplinks_title_models_volkswagen { get; set; }
        public string page_home_toplinks_title_motorbike_makes { get; set; }
        public string page_home_toplinks_title_other_vehicles { get; set; }
        public string page_home_toplinks_title_popular_locations { get; set; }
        public string page_home_toplinks_title_visited { get; set; }

        [JsonProperty("pir-ad-details-coming-soon")]
        public string piraddetailscomingsoon { get; set; }

        [JsonProperty("search-now")]
        public string searchnow { get; set; }

        [JsonProperty("see-selling-options")]
        public string seesellingoptions { get; set; }

        [JsonProperty("sell-2-4-weeks")]
        public string sell24weeks { get; set; }

        [JsonProperty("start-selling")]
        public string startselling { get; set; }

        [JsonProperty("tab-buy")]
        public string tabbuy { get; set; }

        [JsonProperty("tab-sell")]
        public string tabsell { get; set; }

        [JsonProperty("tab-value-my-car")]
        public string tabvaluemycar { get; set; }

        [JsonProperty("Write to us")]
        public string Writetous { get; set; }

        [JsonProperty("listing-filters-range-to-suffix")]
        public string listingfiltersrangetosuffix { get; set; }

        [JsonProperty("listing-filters-range-from-suffix")]
        public string listingfiltersrangefromsuffix { get; set; }

        [JsonProperty("filters-apply-options")]
        public string filtersapplyoptions { get; set; }

        [JsonProperty("filters-clear-options")]
        public string filtersclearoptions { get; set; }

        [JsonProperty("listing-filters-none-select")]
        public string listingfiltersnoneselect { get; set; }

        [JsonProperty("listing-filters-select-all")]
        public string listingfiltersselectall { get; set; }

        [JsonProperty("sell-section-button-3")]
        public string sellsectionbutton3 { get; set; }

        [JsonProperty("sell-section-title")]
        public string sellsectiontitle { get; set; }

        [JsonProperty("sell-section-button-1")]
        public string sellsectionbutton1 { get; set; }

        [JsonProperty("sell-section-button-2")]
        public string sellsectionbutton2 { get; set; }
    }

    public class HomepageRedesign
    {
        public object variant { get; set; }
        public bool isEnabled { get; set; }
    }

    public class HomepageRedesignMweb
    {
        public object variant { get; set; }
        public bool isEnabled { get; set; }
    }

    public class ListingRelevanceExperiments
    {
        public bool enabled { get; set; }
    }

    public class MatchMedia
    {
        public bool isMobile { get; set; }
        public bool isTablet { get; set; }
        public bool isDesktop { get; set; }
    }

    public class Namespaces
    {
        public Home home { get; set; }
        public PricingInsights pricing_insights { get; set; }
        public PageHome page_home { get; set; }
    }

    public class NewCarsAdvertEntrypoint
    {
        public bool enable { get; set; }
    }

    public class NewCarsAdvertRedirectEntrypoint
    {
        public bool enable { get; set; }
    }

    public class NewCarsLeadsManagement
    {
        public List<string> availableSellers { get; set; }
        public bool enable { get; set; }
    }

    public class NewCarsListingEntrypoint
    {
        public List<string> availableSellers { get; set; }
        public bool enable { get; set; }
        public List<string> availableMakes { get; set; }
    }

    public class OfferOfTheDay
    {
        public string id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public Price price { get; set; }
        public List<Characteristic> characteristics { get; set; }
        public string link { get; set; }
    }

    public class OptimusContextProps
    {
        public FeatureFlags featureFlags { get; set; }
        public string experimentCookie { get; set; }
        public List<PendingCookie> pendingCookies { get; set; }
        public CookieComplianceReservoir cookieComplianceReservoir { get; set; }
        public bool isInAppBrowser { get; set; }
        public MatchMedia matchMedia { get; set; }
        public string amazonCloudFrontId { get; set; }
    }

    public class Options
    {
        public int expires { get; set; }
        public string domain { get; set; }
        public string path { get; set; }
    }

    public class PageHome
    {
        [JsonProperty("filter-range-from-suffix")]
        public string filterrangefromsuffix { get; set; }

        [JsonProperty("filter-range-to-suffix")]
        public string filterrangetosuffix { get; set; }

        [JsonProperty("home-advanced-search-button")]
        public string homeadvancedsearchbutton { get; set; }

        [JsonProperty("home-filter-make-all")]
        public string homefiltermakeall { get; set; }

        [JsonProperty("home-filter-make-popular")]
        public string homefiltermakepopular { get; set; }

        [JsonProperty("home-filters-error")]
        public string homefilterserror { get; set; }

        [JsonProperty("home-filters-error-retry")]
        public string homefilterserrorretry { get; set; }

        [JsonProperty("home-search-title")]
        public string homesearchtitle { get; set; }

        [JsonProperty("home-show-results-button")]
        public string homeshowresultsbutton { get; set; }

        [JsonProperty("home-tab-buy-title")]
        public string hometabbuytitle { get; set; }

        [JsonProperty("home-tab-sell-title")]
        public string hometabselltitle { get; set; }

        [JsonProperty("home-view-all-link")]
        public string homeviewalllink { get; set; }

        [JsonProperty("homepage-location-confirm-button")]
        public string homepagelocationconfirmbutton { get; set; }

        [JsonProperty("listing-clear-location-button")]
        public string listingclearlocationbutton { get; set; }

        [JsonProperty("listing-filters-select-all")]
        public string listingfiltersselectall { get; set; }

        [JsonProperty("listing-group-location-caption-selected-city")]
        public string listinggrouplocationcaptionselectedcity { get; set; }

        [JsonProperty("listing-group-location-caption-selected-region")]
        public string listinggrouplocationcaptionselectedregion { get; set; }

        [JsonProperty("listing-location-distance")]
        public string listinglocationdistance { get; set; }

        [JsonProperty("listing-location-placeholder")]
        public string listinglocationplaceholder { get; set; }

        [JsonProperty("listing-see-all-ads-link")]
        public string listingseealladslink { get; set; }

        [JsonProperty("listing-select-placeholder")]
        public string listingselectplaceholder { get; set; }

        [JsonProperty("listing-show-results-button")]
        public string listingshowresultsbutton { get; set; }

        [JsonProperty("resume-last-search-all")]
        public string resumelastsearchall { get; set; }

        [JsonProperty("resume-last-search-count")]
        public ResumeLastSearchCount resumelastsearchcount { get; set; }

        [JsonProperty("resume-last-search-title")]
        public string resumelastsearchtitle { get; set; }

        [JsonProperty("create-ad-otomoto-inspection-learn-more")]
        public string createadotomotoinspectionlearnmore { get; set; }

        [JsonProperty("listing-filters-none-select")]
        public string listingfiltersnoneselect { get; set; }

        [JsonProperty("listing-filters-range-from-suffix")]
        public string listingfiltersrangefromsuffix { get; set; }

        [JsonProperty("listing-filters-range-to-suffix")]
        public string listingfiltersrangetosuffix { get; set; }

        [JsonProperty("filters-apply-options")]
        public string filtersapplyoptions { get; set; }

        [JsonProperty("filters-clear-options")]
        public string filtersclearoptions { get; set; }

        [JsonProperty("sell-section-button-1")]
        public string sellsectionbutton1 { get; set; }

        [JsonProperty("homepage-search-dynamic-title-oto-cars")]
        public string homepagesearchdynamictitleotocars { get; set; }

        [JsonProperty("homepage-search-dynamic-title-oto-default")]
        public string homepagesearchdynamictitleotodefault { get; set; }

        [JsonProperty("homepage-search-dynamic-title-stv")]
        public string homepagesearchdynamictitlestv { get; set; }

        [JsonProperty("homepage-search-dynamic-title-aut")]
        public string homepagesearchdynamictitleaut { get; set; }

        [JsonProperty("sell-section-title")]
        public string sellsectiontitle { get; set; }

        [JsonProperty("sell-section-button-2")]
        public string sellsectionbutton2 { get; set; }

        [JsonProperty("sell-section-button-3")]
        public string sellsectionbutton3 { get; set; }

        [JsonProperty("new-home-advanced-search-button")]
        public string newhomeadvancedsearchbutton { get; set; }
    }

    public class PageProps
    {
        public Category category { get; set; }
        public List<Category> categories { get; set; }
        public OfferOfTheDay offerOfTheDay { get; set; }
        public object banners { get; set; }
        public List<PromotedAd> promotedAds { get; set; }
        public ScreenComponentsFilters screenComponentsFilters { get; set; }
        public AdvertSearchSummary advertSearchSummary { get; set; }
        public Experiments experiments { get; set; }
        public UrqlState urqlState { get; set; }
        public object urqlClient { get; set; }
        public string _sentryTraceData { get; set; }
        public string _sentryBaggage { get; set; }
    }

    public class PendingCookie
    {
        public string name { get; set; }
        public string value { get; set; }
        public Options options { get; set; }
    }

    public class PhysicalInspectionReportRegionsRestriction
    {
        public bool enable { get; set; }
        public List<int> regionIds { get; set; }
    }

    public class Price
    {
        public string currency { get; set; }
        public string value { get; set; }
        public string grossNet { get; set; }
    }

    public class PriceDropExperimentOptOut
    {
        public bool enable { get; set; }
        public List<string> excludedSellers { get; set; }
        public string siteCode { get; set; }
    }

    public class PricingInsights
    {
        [JsonProperty("carval-a-few-more-details")]
        public string carvalafewmoredetails { get; set; }

        [JsonProperty("carval-business-user-label")]
        public string carvalbusinessuserlabel { get; set; }

        [JsonProperty("carval-car-details")]
        public string carvalcardetails { get; set; }

        [JsonProperty("carval-continue")]
        public string carvalcontinue { get; set; }

        [JsonProperty("carval-edit")]
        public string carvaledit { get; set; }

        [JsonProperty("carval-genuine-buyers")]
        public string carvalgenuinebuyers { get; set; }

        [JsonProperty("carval-get-321-offer")]
        public string carvalget321offer { get; set; }

        [JsonProperty("carval-get-a-free-valuation")]
        public string carvalgetafreevaluation { get; set; }

        [JsonProperty("carval-get-a-free-valuation-in-minutes")]
        public string carvalgetafreevaluationinminutes { get; set; }

        [JsonProperty("carval-get-another-valuation")]
        public string carvalgetanothervaluation { get; set; }

        [JsonProperty("carval-great-price")]
        public string carvalgreatprice { get; set; }

        [JsonProperty("carval-how-much-is-your-car-worth")]
        public string carvalhowmuchisyourcarworth { get; set; }

        [JsonProperty("carval-illustration-description")]
        public string carvalillustrationdescription { get; set; }

        [JsonProperty("carval-instant-offer")]
        public string carvalinstantoffer { get; set; }

        [JsonProperty("carval-not-enough")]
        public string carvalnotenough { get; set; }

        [JsonProperty("carval-post-an-ad")]
        public string carvalpostanad { get; set; }

        [JsonProperty("carval-private-sale")]
        public string carvalprivatesale { get; set; }

        [JsonProperty("carval-private-user-label")]
        public string carvalprivateuserlabel { get; set; }

        [JsonProperty("carval-required-information")]
        public string carvalrequiredinformation { get; set; }

        [JsonProperty("carval-safe-money-transfer")]
        public string carvalsafemoneytransfer { get; set; }

        [JsonProperty("carval-search-for-similar-cars")]
        public string carvalsearchforsimilarcars { get; set; }

        [JsonProperty("carval-sell-in-24-hours")]
        public string carvalsellin24hours { get; set; }

        [JsonProperty("carval-sell-in-weeks")]
        public string carvalsellinweeks { get; set; }

        [JsonProperty("carval-steps")]
        public string carvalsteps { get; set; }

        [JsonProperty("carval-take-care-of-paperwork")]
        public string carvaltakecareofpaperwork { get; set; }

        [JsonProperty("carval-tools-to-help")]
        public string carvaltoolstohelp { get; set; }

        [JsonProperty("carval-unfortunately")]
        public string carvalunfortunately { get; set; }

        [JsonProperty("carval-what-type-of-user")]
        public string carvalwhattypeofuser { get; set; }

        [JsonProperty("carval-your-valuation-is-ready")]
        public string carvalyourvaluationisready { get; set; }

        [JsonProperty("create-an-account")]
        public string createanaccount { get; set; }

        [JsonProperty("demand-notification")]
        public string demandnotification { get; set; }

        [JsonProperty("dropdown-placeholder")]
        public string dropdownplaceholder { get; set; }

        [JsonProperty("good-news-quote-car")]
        public string goodnewsquotecar { get; set; }

        [JsonProperty("log-in-find-how-much-car-worth")]
        public string loginfindhowmuchcarworth { get; set; }

        [JsonProperty("log-in-to-see-valuation")]
        public string logintoseevaluation { get; set; }

        [JsonProperty("not-a-number-error-message")]
        public string notanumbererrormessage { get; set; }

        [JsonProperty("pe-badge-bubble-label-above")]
        public string pebadgebubblelabelabove { get; set; }

        [JsonProperty("pe-badge-bubble-label-below")]
        public string pebadgebubblelabelbelow { get; set; }

        [JsonProperty("pe-badge-bubble-label-in")]
        public string pebadgebubblelabelin { get; set; }

        [JsonProperty("pes-badge-bubble-label-in")]
        public string pesbadgebubblelabelin { get; set; }

        [JsonProperty("value-required-error-message")]
        public string valuerequirederrormessage { get; set; }

        [JsonProperty("tab-post-an-ad")]
        public string tabpostanad { get; set; }

        [JsonProperty("pir-create-ad-inspection-learn-more")]
        public string pircreateadinspectionlearnmore { get; set; }

        [JsonProperty("how-much-car-worth")]
        public string howmuchcarworth { get; set; }

        [JsonProperty("get-a-free-estimate")]
        public string getafreeestimate { get; set; }

        [JsonProperty("ads-posted-day")]
        public string adspostedday { get; set; }

        [JsonProperty("set-your-price-confidence")]
        public string setyourpriceconfidence { get; set; }

        [JsonProperty("ads-used-calculate-rate")]
        public string adsusedcalculaterate { get; set; }

        [JsonProperty("users-found-valuation-useful")]
        public string usersfoundvaluationuseful { get; set; }

        [JsonProperty("identification-title")]
        public string identificationtitle { get; set; }

        [JsonProperty("identification-description")]
        public string identificationdescription { get; set; }

        [JsonProperty("analysis-title")]
        public string analysistitle { get; set; }

        [JsonProperty("calculation-title")]
        public string calculationtitle { get; set; }

        [JsonProperty("how-is-car-calculated")]
        public string howiscarcalculated { get; set; }

        [JsonProperty("example-valuation-range")]
        public string examplevaluationrange { get; set; }

        [JsonProperty("analysis-description")]
        public string analysisdescription { get; set; }

        [JsonProperty("calculation-description")]
        public string calculationdescription { get; set; }

        [JsonProperty("prepare-car-to-sell-2")]
        public string preparecartosell2 { get; set; }

        [JsonProperty("prepare-car-to-sell-1")]
        public string preparecartosell1 { get; set; }

        [JsonProperty("prepare-car-to-sell")]
        public string preparecartosell { get; set; }

        [JsonProperty("prepare-car-to-sell-3")]
        public string preparecartosell3 { get; set; }

        [JsonProperty("prepare-car-to-sell-4")]
        public string preparecartosell4 { get; set; }

        [JsonProperty("prepare-car-to-sell-5")]
        public string preparecartosell5 { get; set; }

        [JsonProperty("your-questions-answered")]
        public string yourquestionsanswered { get; set; }

        [JsonProperty("faq-2-title")]
        public string faq2title { get; set; }

        [JsonProperty("faq-4-title")]
        public string faq4title { get; set; }

        [JsonProperty("faq-3-title")]
        public string faq3title { get; set; }

        [JsonProperty("faq-1-title")]
        public string faq1title { get; set; }

        [JsonProperty("faq-4-description")]
        public string faq4description { get; set; }

        [JsonProperty("faq-1-description")]
        public string faq1description { get; set; }

        [JsonProperty("faq-2-description")]
        public string faq2description { get; set; }

        [JsonProperty("faq-3-description")]
        public string faq3description { get; set; }

        [JsonProperty("free-car-valautions")]
        public string freecarvalautions { get; set; }

        [JsonProperty("carval-how-much-is-your-car-worth-short")]
        public string carvalhowmuchisyourcarworthshort { get; set; }

        [JsonProperty("carval-get-a-free-quote")]
        public string carvalgetafreequote { get; set; }

        [JsonProperty("new-listing-cvt-title")]
        public string newlistingcvttitle { get; set; }

        [JsonProperty("new-listing-cvt-cta")]
        public string newlistingcvtcta { get; set; }

        [JsonProperty("new-listing-cvt-badge-text")]
        public string newlistingcvtbadgetext { get; set; }
    }

    public class PromotedAd
    {
        public string id { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public Price price { get; set; }
        public List<Characteristic> characteristics { get; set; }
        public string link { get; set; }
    }

    public class Props
    {
        public PageProps pageProps { get; set; }
        public object session { get; set; }

        [JsonProperty("$_optimusContextProps")]
        public OptimusContextProps _optimusContextProps { get; set; }
        public string Server_routeId { get; set; }
        public ServerCookieSize serverCookieSize { get; set; }
        public string emotionNonce { get; set; }
        public string __lang { get; set; }
        public Namespaces __namespaces { get; set; }
    }

    public class Query
    {
    }

    public class ResumeLastSearchCount
    {
        public string zero { get; set; }
        public string one { get; set; }
        public string other { get; set; }
    }

    public class FilteresModel
    {
        public Props props { get; set; }
        public string page { get; set; }
        public Query query { get; set; }
        public string buildId { get; set; }
        public string assetPrefix { get; set; }
        public RuntimeConfig runtimeConfig { get; set; }
        public bool isFallback { get; set; }
        public bool isExperimentalCompile { get; set; }
        public List<int> dynamicIds { get; set; }
        public bool gip { get; set; }
        public bool appGip { get; set; }
        public string locale { get; set; }
        public List<string> locales { get; set; }
        public string defaultLocale { get; set; }
        public List<ScriptLoader> scriptLoader { get; set; }
    }

    public class RuntimeConfig
    {
        public string version { get; set; }
    }

    public class ScreenComponentsFilters
    {
        public string __typename { get; set; }
        public List<Component> components { get; set; }
        public List<State> states { get; set; }
    }

    public class ScriptLoader
    {
        public string id { get; set; }
        public List<object> children { get; set; }
        public string strategy { get; set; }
    }

    public class ServerCookieSize
    {
        public int ldf { get; set; }
    }

    public class SourcingFiltersLayoutFix
    {
        public bool enabled { get; set; }
    }

    public class State
    {
        public string __typename { get; set; }
        public string filterId { get; set; }
        public List<object> defaultSelectedValues { get; set; }
        public List<Condition> conditions { get; set; }
        public List<Value> values { get; set; }
    }

    public class UrqlState
    {
    }

    public class Value
    {
        public string __typename { get; set; }
        public object name { get; set; }
        public List<SubValue> values { get; set; }
       
    }


    public class SubValue
    {
        public string __typename { get; set; }
        public string? Id { get; set; }
        public string? name { get; set; }
        public string description { get; set; }
        public int? counter { get; set; }
    }

   

